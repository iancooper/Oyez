namespace Oyez.Core.Announcements;

/// <summary>
/// Keeps announcements in memory. Use it in tests, or wherever announcements don't come from files.
/// </summary>
public sealed class InMemoryAnnouncementStore : IAmAnAnnouncementStore
{
    private readonly Dictionary<AnnouncementId, AnnouncementDocument> _documents = new();

    /// <summary>Adds an announcement, replacing any with the same ID.</summary>
    /// <param name="document">The <see cref="AnnouncementDocument"/> to add.</param>
    public void Add(AnnouncementDocument document) => _documents[document.Id] = document;

    /// <inheritdoc />
    public IReadOnlyList<AnnouncementId> List() => _documents.Keys.ToList();

    /// <inheritdoc />
    public AnnouncementDocument? Load(AnnouncementId id) => _documents.GetValueOrDefault(id);
}
