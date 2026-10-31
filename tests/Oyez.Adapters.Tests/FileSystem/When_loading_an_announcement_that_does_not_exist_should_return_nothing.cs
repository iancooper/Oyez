using Oyez.Core.Announcements;

namespace Oyez.Adapters.Tests.FileSystem;

public class LoadingMissingAnnouncementTests
{
    [Fact]
    public void When_loading_an_announcement_that_does_not_exist_should_return_nothing()
    {
        //Arrange
        using var folder = new TemporaryFolder();
        folder.WriteFile("2026-11-03-brighter-10-3-0/announcement.txt", "Brighter 10.3.0 is out.");
        using var host = new OyezAdaptersHost(options => options.AnnouncementsPath = folder.Path);
        var store = host.Service<IAmAnAnnouncementStore>();

        //Act
        var document = store.Load(new AnnouncementId("2026-12-01-no-such-announcement"));

        //Assert
        Assert.Null(document);
    }
}
