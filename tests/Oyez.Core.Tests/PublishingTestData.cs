using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests;

internal static class PublishingTestData
{
    public static AnnouncementDocument Announcement(AnnouncementId id, string channels, string[] segments, string? publish = null) =>
        new(id, AnnouncementText.With(channels, segments, publish), WriteAsMarkdown: null);

    public static PublicationRecord RecordWith(params (ChannelId Channel, ChannelPublication Publication)[] channels) =>
        new(channels.ToDictionary(c => c.Channel, c => c.Publication));
}
