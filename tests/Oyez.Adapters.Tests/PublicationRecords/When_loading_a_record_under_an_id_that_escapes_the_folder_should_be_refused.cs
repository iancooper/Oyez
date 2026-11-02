using Oyez.Core.Announcements;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.PublicationRecords;

public class EscapingRecordIdLoadTests
{
    [Fact]
    public void When_loading_a_record_under_an_id_that_escapes_the_folder_should_be_refused()
    {
        //Arrange
        using var folder = new TemporaryFolder();
        var announcements = Path.Combine(folder.Path, "announcements");
        Directory.CreateDirectory(announcements);
        using var host = new OyezAdaptersHost(options => options.AnnouncementsPath = announcements);
        var store = host.Service<IAmAPublicationRecordStore>();
        var escape = new AnnouncementId("../outside");

        //Act
        var failure = Record.Exception(() => store.Load(escape));

        //Assert
        Assert.IsType<ArgumentException>(failure);
    }
}
