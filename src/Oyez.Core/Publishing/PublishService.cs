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

namespace Oyez.Core.Publishing;

internal sealed class PublishService(
    IAmAnAnnouncementStore announcements,
    Renderer renderer,
    Validator validator,
    ChannelRunFactory runs,
    TimeProvider clock)
{
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
        var outcomes = new List<PublishOutcome?>();
        foreach (var rendering in renderer.RenderTargetedChannels(announcement))
        {
            outcomes.Add(await runs.Create(announcement.Id, rendering).PublishAsync(cancellationToken));
        }

        return outcomes.OfType<PublishOutcome>().ToList();
    }
}
