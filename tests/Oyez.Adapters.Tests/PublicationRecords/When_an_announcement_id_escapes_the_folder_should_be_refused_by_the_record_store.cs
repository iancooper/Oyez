using Oyez.Core.Announcements;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.PublicationRecords;

public class EscapingRecordIdTests
{
    [Fact]
    public void When_an_announcement_id_escapes_the_folder_should_be_refused_by_the_record_store()
    {
        //Arrange
        using var folder = new TemporaryFolder();
        var announcements = Path.Combine(folder.Path, "announcements");
        Directory.CreateDirectory(announcements);
        using var host = new OyezAdaptersHost(options => options.AnnouncementsPath = announcements);
        var store = host.Service<IAmAPublicationRecordStore>();
        var record = PublicationRecordTestData.RecordWithEveryState();
        var relativeEscape = new AnnouncementId("../outside");
        var absoluteEscape = new AnnouncementId(Path.Combine(folder.Path, "absolute-outside"));

        //Act
        var relativeFailure = Record.Exception(() => store.Save(relativeEscape, record));
        var absoluteFailure = Record.Exception(() => store.Save(absoluteEscape, record));
        var loadFailure = Record.Exception(() => store.Load(relativeEscape));

        //Assert
        Assert.IsType<ArgumentException>(relativeFailure);
        Assert.IsType<ArgumentException>(absoluteFailure);
        Assert.IsType<ArgumentException>(loadFailure);
        Assert.False(Directory.Exists(Path.Combine(folder.Path, "outside")));
        Assert.False(Directory.Exists(Path.Combine(folder.Path, "absolute-outside")));
    }
}
