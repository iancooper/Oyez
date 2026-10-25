using Oyez.Core.Announcements;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests;

public sealed class RecordingPublicationRecordStore : IAmAPublicationRecordStore
{
    private readonly InMemoryPublicationRecordStore _inner = new();
    private readonly List<PublicationRecord> _saves = [];

    public IReadOnlyList<PublicationRecord> Saves => _saves;

    public PublicationRecord Load(AnnouncementId id) => _inner.Load(id);

    public void Save(AnnouncementId id, PublicationRecord record)
    {
        _saves.Add(record);
        _inner.Save(id, record);
    }
}
