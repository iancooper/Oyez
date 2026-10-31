#region Licence

/* The MIT License (MIT)
Copyright © 2026 Ian Cooper

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE. */

#endregion

using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Paramore.Brighter;

namespace Oyez.Core.Publishing;

internal sealed class ChannelRun(
    AnnouncementId id,
    Rendering rendering,
    IAmAChannelPublisher publisher,
    ChannelPublication? recorded,
    IAmAPublicationRecordStore records,
    IAmACommandProcessor commands,
    TimeProvider clock)
{
    private const string EDITED_REASON = "The announcement was edited after it was partly published. Resolve the channel before publishing again.";

    private ChannelId Channel => rendering.Channel;

    public async Task<PublishOutcome?> PublishAsync(CancellationToken cancellationToken)
    {
        if (recorded is { State: ChannelState.Complete })
        {
            return null;
        }

        if (recorded is { State: ChannelState.Indeterminate })
        {
            return OutcomeOf(recorded, recorded.Posts.Count);
        }

        if (recorded?.IsEditedSince(rendering) == true)
        {
            return await FailEditedAsync(recorded, cancellationToken);
        }

        return await PostAllAsync(cancellationToken);
    }

    private async Task<PublishOutcome> FailEditedAsync(ChannelPublication edited, CancellationToken cancellationToken)
    {
        var failed = await StopAsync(edited.Fail(EDITED_REASON), cancellationToken);
        return OutcomeOf(failed, edited.Posts.Count);
    }

    private async Task<PublishOutcome> PostAllAsync(CancellationToken cancellationToken)
    {
        var publication = (recorded ?? ChannelPublication.Begin(rendering)).ResumeFor(rendering);
        var alreadyPosted = publication.Posts.Count;
        while (publication.HasPostsToMake(rendering))
        {
            publication = await PostNextAsync(publication, cancellationToken);
        }

        return OutcomeOf(publication, alreadyPosted);
    }

    private async Task<ChannelPublication> PostNextAsync(ChannelPublication publication, CancellationToken cancellationToken)
    {
        var index = publication.Posts.Count;
        var request = new PostRequest(id, rendering, index, publication.Posts.LastOrDefault()?.Ref, IdempotencyKey(index));
        var result = await publisher.CreateAsync(request, cancellationToken);

        return result switch
        {
            PostResult.Posted posted => await AcceptAsync(publication, posted, cancellationToken),
            PostResult.Rejected rejected => await StopAsync(publication.Fail(rejected.Reason), cancellationToken),
            PostResult.Indeterminate indeterminate => await StopAsync(publication.MarkIndeterminate(indeterminate.Reason), cancellationToken),
            _ => throw new InvalidOperationException($"Unknown post result {result}")
        };
    }

    private async Task<ChannelPublication> AcceptAsync(ChannelPublication publication, PostResult.Posted posted, CancellationToken cancellationToken)
    {
        var index = publication.Posts.Count;
        var accepted = publication.Append(posted.Ref, clock.GetUtcNow(), rendering);
        Save(accepted);
        await commands.PublishAsync(new PostPublished(id, Channel, index, posted.Ref), cancellationToken: cancellationToken);
        return accepted;
    }

    private async Task<ChannelPublication> StopAsync(ChannelPublication stopped, CancellationToken cancellationToken)
    {
        Save(stopped);
        await commands.PublishAsync(new ChannelPublishFailed(id, Channel, stopped.State, stopped.Reason ?? string.Empty), cancellationToken: cancellationToken);
        return stopped;
    }

    private string IdempotencyKey(int index) =>
        $"oyez:{id}:{Channel}:{index}:{rendering.Hash.Fingerprint}";

    private void Save(ChannelPublication publication) =>
        records.SaveChannel(id, Channel, publication);

    private PublishOutcome OutcomeOf(ChannelPublication publication, int alreadyPosted) =>
        new(id, Channel, publication.State, publication.Reason, publication.Posts.Skip(alreadyPosted).Select(post => post.Ref).ToList());
}
