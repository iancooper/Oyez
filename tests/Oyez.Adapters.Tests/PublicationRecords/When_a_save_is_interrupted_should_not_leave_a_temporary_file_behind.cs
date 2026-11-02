using Oyez.Core.Announcements;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.PublicationRecords;

public class InterruptedSavingRecordTests
{
    [Fact]
    public void When_a_save_is_interrupted_should_not_leave_a_temporary_file_behind()
    {
        //Arrange
        using var folder = new TemporaryFolder();
        var id = new AnnouncementId("2026-11-03-brighter-10-3-0");
        var announcementFolder = Path.Combine(folder.Path, id.Value);
        Directory.CreateDirectory(Path.Combine(announcementFolder, "published.json"));
        using var host = new OyezAdaptersHost(options => options.AnnouncementsPath = folder.Path);
        var store = host.Service<IAmAPublicationRecordStore>();

        //Act
        var failure = Record.Exception(() => store.Save(id, PublicationRecordTestData.RecordWithEveryState()));

        //Assert
        Assert.NotNull(failure);
        Assert.Empty(Directory.GetFiles(announcementFolder));
    }
}
