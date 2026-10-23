using Oyez.Core.Announcements;
using Paramore.Darker;

namespace Oyez.Core.Previewing;

/// <summary>
/// Asks how an announcement will look on each channel it targets, without publishing anything.
/// </summary>
/// <param name="Id">The <see cref="AnnouncementId"/> of the announcement to preview.</param>
public sealed record PreviewAnnouncement(AnnouncementId Id) : IQuery<Result<Preview>>;
