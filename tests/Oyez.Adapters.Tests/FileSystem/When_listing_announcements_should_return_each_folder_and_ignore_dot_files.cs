using Oyez.Core.Announcements;

namespace Oyez.Adapters.Tests.FileSystem;

public class ListingAnnouncementsTests
{
    [Fact]
    public void When_listing_announcements_should_return_each_folder_and_ignore_dot_files()
    {
        //Arrange
        using var folder = new TemporaryFolder();
        folder.WriteFile("2026-11-03-brighter-10-3-0/announcement.txt", "text");
        folder.WriteFile("2026-10-01-brighter-10-2-0/announcement.txt", "text");
        folder.WriteFile(".drafted.json", "{}");
        folder.WriteFile(".git/announcement.txt", "text");
        using var host = new OyezAdaptersHost(options => options.AnnouncementsPath = folder.Path);
        var store = host.Service<IAmAnAnnouncementStore>();

        //Act
        var ids = store.List();

        //Assert
        Assert.Equal(
            [new AnnouncementId("2026-10-01-brighter-10-2-0"), new AnnouncementId("2026-11-03-brighter-10-3-0")],
            ids);
    }
}
