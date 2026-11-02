using Oyez.Core.Announcements;

namespace Oyez.Adapters.Tests.FileSystem;

public class EscapingAnnouncementIdTests
{
    [Fact]
    public void When_an_announcement_id_escapes_the_folder_should_be_refused_by_the_announcement_store()
    {
        //Arrange
        using var folder = new TemporaryFolder();
        folder.WriteFile("outside/announcement.txt", "Not an announcement.");
        Directory.CreateDirectory(Path.Combine(folder.Path, "announcements"));
        using var host = new OyezAdaptersHost(options => options.AnnouncementsPath = Path.Combine(folder.Path, "announcements"));
        var store = host.Service<IAmAnAnnouncementStore>();
        var relativeEscape = new AnnouncementId("../outside");
        var absoluteEscape = new AnnouncementId(Path.Combine(folder.Path, "outside"));

        //Act
        var relativeResult = store.Load(relativeEscape);
        var absoluteResult = store.Load(absoluteEscape);

        //Assert
        Assert.Null(relativeResult);
        Assert.Null(absoluteResult);
    }
}
