using Oyez.Core.Announcements;

namespace Oyez.Adapters.Tests.FileSystem;

public class LoadingAnnouncementTests
{
    [Fact]
    public void When_loading_an_announcement_should_return_its_text_and_markdown()
    {
        //Arrange
        using var folder = new TemporaryFolder();
        var id = new AnnouncementId("2026-11-03-brighter-10-3-0");
        folder.WriteFile($"{id}/announcement.txt", "Brighter 10.3.0 is out.");
        folder.WriteFile($"{id}/writeas.md", "# Brighter 10.3.0");
        using var host = new OyezAdaptersHost(options => options.AnnouncementsPath = folder.Path);
        var store = host.Service<IAmAnAnnouncementStore>();

        //Act
        var document = store.Load(id);

        //Assert
        Assert.Equal(new AnnouncementDocument(id, "Brighter 10.3.0 is out.", "# Brighter 10.3.0"), document);
    }
}
