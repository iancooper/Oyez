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
        var due = DueAnnouncements().ToList();
        var publishable = due.Where(candidate => candidate.Errors.Count == 0).Select(candidate => candidate.Announcement).OfType<Announcement>();
        var outcomes = new List<PublishOutcome>();
        foreach (var announcement in publishable)
        {
            outcomes.AddRange(await PublishAnnouncementAsync(announcement, cancellationToken));
        }

        return new PublishReport(outcomes, due.Where(candidate => candidate.Errors.Count > 0).Select(NotPublished).ToList());
    }

    private static UnpublishedAnnouncement NotPublished(DueAnnouncement candidate) => new(candidate.Id, candidate.Errors);

    private IEnumerable<DueAnnouncement> DueAnnouncements() =>
        announcements.List()
            .Select(id => (Id: id, Read: announcements.ReadAnnouncement(id)))
            .Where(item => !item.Read.IsSuccess || IsDue(item.Read.Value))
            .Select(item => item.Read.IsSuccess ? Validated(item.Id, item.Read.Value) : new DueAnnouncement(item.Id, null, item.Read.Errors));

    private DueAnnouncement Validated(AnnouncementId id, Announcement announcement) =>
        new(id, announcement, validator.Validate(announcement));

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

    private sealed record DueAnnouncement(AnnouncementId Id, Announcement? Announcement, IReadOnlyList<ValidationError> Errors);
}
