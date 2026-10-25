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
using ValidationError = Oyez.Core.Announcements.ValidationError;

namespace Oyez.Core.Publishing;

internal sealed class ResolveChannelHandler(
    IAmAnAnnouncementStore announcements,
    IAmAPublicationRecordStore records,
    Renderer renderer,
    TimeProvider clock) : RequestHandlerAsync<ResolveChannel>
{
    private const string NOT_POSTED_REASON = "Resolved as not posted, so the post will be retried.";

    public override Task<ResolveChannel> HandleAsync(ResolveChannel command, CancellationToken cancellationToken = default)
    {
        command.Result = Resolve(command);
        return base.HandleAsync(command, cancellationToken);
    }

    private Result<ChannelState> Resolve(ResolveChannel command)
    {
        var record = records.Load(command.Announcement);
        if (!record.Channels.TryGetValue(command.Channel, out var publication) || publication.State != ChannelState.Indeterminate)
        {
            return Failure(command, $"{command.Channel.DisplayName} is not indeterminate, so there is nothing to resolve.");
        }

        var read = announcements.ReadAnnouncement(command.Announcement);
        var rendering = read.IsSuccess ? renderer.RenderTargetedChannels(read.Value).FirstOrDefault(candidate => candidate.Channel == command.Channel) : null;
        if (rendering is null)
        {
            return Failure(command, $"{command.Channel.DisplayName} is not a channel of announcement '{command.Announcement}'.");
        }

        var resolved = command.Posted is null ? publication.Fail(NOT_POSTED_REASON) : publication.Append(command.Posted, clock.GetUtcNow(), rendering) with { Reason = null };
        Save(command, record, resolved);
        return Result<ChannelState>.Success(resolved.State);
    }

    private static Result<ChannelState> Failure(ResolveChannel command, string message) =>
        Result<ChannelState>.Failure([new ValidationError(command.Announcement, message, command.Channel)]);

    private void Save(ResolveChannel command, PublicationRecord record, ChannelPublication resolved)
    {
        records.Save(command.Announcement, record.With(command.Channel, resolved));
    }
}
