using Oyez.Core.Channels;

namespace Oyez.Core.Announcements;

internal sealed record Announcement(
    AnnouncementId Id,
    string Title,
    DateTimeOffset? PublishAt,
    IReadOnlySet<ChannelId> Channels,
    IReadOnlyList<Segment> Segments,
    string? WriteAsMarkdown);
