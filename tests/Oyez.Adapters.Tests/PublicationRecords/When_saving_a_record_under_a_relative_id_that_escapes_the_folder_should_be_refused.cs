using Oyez.Core.Announcements;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.PublicationRecords;

public class EscapingRelativeRecordIdSaveTests
{
    [Fact]
    public void When_saving_a_record_under_a_relative_id_that_escapes_the_folder_should_be_refused()
    {
        //Arrange
        using var folder = new TemporaryFolder();
        var announcements = Path.Combine(folder.Path, "announcements");
        Directory.CreateDirectory(announcements);
        using var host = new OyezAdaptersHost(options => options.AnnouncementsPath = announcements);
        var store = host.Service<IAmAPublicationRecordStore>();
        var escape = new AnnouncementId("../outside");

        //Act
        var failure = Record.Exception(() => store.Save(escape, PublicationRecordTestData.RecordWithEveryState()));

        //Assert
        Assert.IsType<ArgumentException>(failure);
        Assert.False(Directory.Exists(Path.Combine(folder.Path, "outside")));
    }
}
