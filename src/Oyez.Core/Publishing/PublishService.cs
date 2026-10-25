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
using Oyez.Core.Validating;
using Paramore.Brighter;

namespace Oyez.Core.Publishing;

internal sealed class PublishService(
    IAmAnAnnouncementStore announcements,
    IAmAPublicationRecordStore records,
    Renderer renderer,
    Validator validator,
    IEnumerable<IAmAChannelPublisher> publishers,
    IAmACommandProcessor commands,
    TimeProvider clock)
{
    private const string EDITED_REASON = "The announcement was edited after it was partly published. Resolve the channel before publishing again.";

    public async Task<PublishReport> PublishDueAsync(CancellationToken cancellationToken)
    {
        var outcomes = new List<PublishOutcome>();
        foreach (var announcement in DueAnnouncements())
        {
            outcomes.AddRange(await PublishAnnouncementAsync(announcement, cancellationToken));
        }

        return new PublishReport(outcomes);
    }

    private IEnumerable<Announcement> DueAnnouncements() =>
        announcements.List()
            .Select(announcements.ReadAnnouncement)
            .Where(read => read.IsSuccess)
            .Select(read => read.Value)
            .Where(IsDue)
            .Where(announcement => validator.Validate(announcement).Count == 0);

    private bool IsDue(Announcement announcement) =>
        announcement.PublishAt is null || announcement.PublishAt <= clock.GetUtcNow();

    private async Task<IReadOnlyList<PublishOutcome>> PublishAnnouncementAsync(Announcement announcement, CancellationToken cancellationToken)
    {
        var outcomes = new List<PublishOutcome>();
        var record = records.Load(announcement.Id);
        foreach (var rendering in renderer.RenderTargetedChannels(announcement).Where(rendering => !record.IsComplete(rendering.Channel)))
        {
            outcomes.Add(await PublishChannelAsync(announcement.Id, rendering, cancellationToken));
        }

        return outcomes;
    }

    private async Task<PublishOutcome> PublishChannelAsync(AnnouncementId id, Rendering rendering, CancellationToken cancellationToken)
    {
        var publisher = publishers.Single(candidate => candidate.Channel == rendering.Channel);
        var recorded = records.Load(id).ChannelOrNull(rendering.Channel);
        if (recorded?.State == ChannelState.Indeterminate)
        {
            return OutcomeOf(id, rendering.Channel, recorded, 0);
        }

        if (recorded?.IsEditedSince(rendering) == true)
        {
            return await FailEditedAsync(id, rendering.Channel, recorded, cancellationToken);
        }

        var publication = (recorded ?? ChannelPublication.Begin(rendering)).ResumeFor(rendering);
        var alreadyPosted = publication.Posts.Count;
        while (publication.HasPostsToMake(rendering))
        {
            var previous = publication;
            publication = await PostNextAsync(id, rendering, publisher, publication, cancellationToken);
            Save(id, rendering.Channel, publication);
            await PublishProgressAsync(id, rendering.Channel, previous, publication, cancellationToken);
        }

        return OutcomeOf(id, rendering.Channel, publication, alreadyPosted);
    }

    private static PublishOutcome OutcomeOf(AnnouncementId id, ChannelId channel, ChannelPublication publication, int alreadyPosted) =>
        new(id, channel, publication.State, publication.Reason, publication.Posts.Skip(alreadyPosted).Select(post => post.Ref).ToList());

    private async Task<PublishOutcome> FailEditedAsync(AnnouncementId id, ChannelId channel, ChannelPublication recorded, CancellationToken cancellationToken)
    {
        var failed = recorded.Fail(EDITED_REASON);
        Save(id, channel, failed);
        await PublishProgressAsync(id, channel, recorded, failed, cancellationToken);
        return OutcomeOf(id, channel, failed, recorded.Posts.Count);
    }

    private async Task PublishProgressAsync(
        AnnouncementId id,
        ChannelId channel,
        ChannelPublication previous,
        ChannelPublication current,
        CancellationToken cancellationToken)
    {
        if (current.Posts.Count > previous.Posts.Count)
        {
            var post = current.Posts[^1];
            await commands.PublishAsync(new PostPublished(id, channel, post.Index, post.Ref), cancellationToken: cancellationToken);
        }
        else if (current.State is ChannelState.Failed or ChannelState.Indeterminate)
        {
            await commands.PublishAsync(new ChannelPublishFailed(id, channel, current.State, current.Reason ?? string.Empty), cancellationToken: cancellationToken);
        }
    }

    private async Task<ChannelPublication> PostNextAsync(
        AnnouncementId id,
        Rendering rendering,
        IAmAChannelPublisher publisher,
        ChannelPublication publication,
        CancellationToken cancellationToken)
    {
        var index = publication.Posts.Count;
        var request = new PostRequest(id, rendering, index, publication.Posts.LastOrDefault()?.Ref, IdempotencyKey(id, rendering, index));
        var result = await publisher.CreateAsync(request, cancellationToken);

        return result switch
        {
            PostResult.Posted posted => publication.Append(posted.Ref, clock.GetUtcNow(), rendering) with { RenderingHash = rendering.Hash },
            PostResult.Rejected rejected => publication.Fail(rejected.Reason),
            PostResult.Indeterminate indeterminate => publication.MarkIndeterminate(indeterminate.Reason),
            _ => throw new InvalidOperationException($"Unknown post result {result}")
        };
    }

    private static string IdempotencyKey(AnnouncementId id, Rendering rendering, int index) =>
        $"oyez:{id}:{rendering.Channel}:{index}:{rendering.Hash.Fingerprint}";

    private void Save(AnnouncementId id, ChannelId channel, ChannelPublication publication)
    {
        records.Save(id, records.Load(id).With(channel, publication));
    }
}
