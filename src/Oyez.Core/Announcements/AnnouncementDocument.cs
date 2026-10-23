namespace Oyez.Core.Announcements;

/// <summary>
/// The raw files that make up an announcement, before Oyez parses them.
/// </summary>
/// <param name="Id">The <see cref="AnnouncementId"/> of the announcement.</param>
/// <param name="Text">The contents of <c>announcement.txt</c>: a header, then segments separated by <c>---</c> lines.</param>
/// <param name="WriteAsMarkdown">The contents of <c>writeas.md</c>, or <c>null</c> if the announcement has none.</param>
public sealed record AnnouncementDocument(AnnouncementId Id, string Text, string? WriteAsMarkdown);
