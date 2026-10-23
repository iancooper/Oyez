using Oyez.Core.Announcements;
using Oyez.Core.Channels;

namespace Oyez.Core.Previewing;

/// <summary>
/// How an announcement will look on each channel it targets.
/// </summary>
/// <param name="Id">The <see cref="AnnouncementId"/> of the announcement.</param>
/// <param name="Title">The announcement's title.</param>
/// <param name="PublishAt">When the announcement is due, in UTC, or <c>null</c> to publish it as soon as possible.</param>
/// <param name="Renderings">One <see cref="Rendering"/> per targeted channel, in the order of <see cref="ChannelId.All"/>.</param>
public sealed record Preview(
    AnnouncementId Id,
    string Title,
    DateTimeOffset? PublishAt,
    IReadOnlyList<Rendering> Renderings);
