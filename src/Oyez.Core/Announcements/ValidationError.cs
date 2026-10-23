using Oyez.Core.Channels;

namespace Oyez.Core.Announcements;

/// <summary>
/// A problem with an announcement, located as precisely as possible.
/// </summary>
/// <param name="Announcement">The <see cref="AnnouncementId"/> of the announcement with the problem.</param>
/// <param name="Message">What is wrong, and how to fix it where that's known.</param>
/// <param name="Channel">The <see cref="ChannelId"/> the problem affects, or <c>null</c> if it affects them all.</param>
/// <param name="Segment">The number of the segment with the problem, counting from 1, or <c>null</c> if it isn't in a segment.</param>
public sealed record ValidationError(
    AnnouncementId Announcement,
    string Message,
    ChannelId? Channel = null,
    int? Segment = null);
