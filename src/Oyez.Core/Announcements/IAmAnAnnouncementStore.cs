namespace Oyez.Core.Announcements;

/// <summary>
/// Reads announcements from where they are kept, such as the <c>announcements/</c> folder.
/// </summary>
/// <remarks>
/// A store only returns the raw <see cref="AnnouncementDocument"/>. Oyez parses and validates it.
/// </remarks>
public interface IAmAnAnnouncementStore
{
    /// <summary>Lists every announcement in the store.</summary>
    /// <returns>The <see cref="AnnouncementId"/> of each announcement.</returns>
    IReadOnlyList<AnnouncementId> List();

    /// <summary>Reads one announcement's files.</summary>
    /// <param name="id">The <see cref="AnnouncementId"/> of the announcement to read.</param>
    /// <returns>The <see cref="AnnouncementDocument"/>, or <c>null</c> if there is no such announcement.</returns>
    AnnouncementDocument? Load(AnnouncementId id);
}
