using Oyez.Core.Announcements;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.PublicationRecords;

public class LoadingRecordTests
{
    [Fact]
    public void When_loading_a_saved_record_should_return_the_same_record()
    {
        //Arrange
        using var folder = new TemporaryFolder();
        var id = new AnnouncementId("2026-11-03-brighter-10-3-0");
        using var host = new OyezAdaptersHost(options => options.AnnouncementsPath = folder.Path);
        var store = host.Service<IAmAPublicationRecordStore>();
        var saved = PublicationRecordTestData.RecordWithEveryState();
        store.Save(id, saved);

        //Act
        var loaded = store.Load(id);

        //Assert
        PublicationRecordTestData.AssertSame(saved, loaded);
    }
}
