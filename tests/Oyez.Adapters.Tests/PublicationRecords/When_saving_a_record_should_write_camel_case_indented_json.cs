using Oyez.Core.Announcements;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.PublicationRecords;

public class SavingRecordTests
{
    [Fact]
    public void When_saving_a_record_should_write_camel_case_indented_json()
    {
        //Arrange
        using var folder = new TemporaryFolder();
        var id = new AnnouncementId("2026-11-03-brighter-10-3-0");
        using var host = new OyezAdaptersHost(options => options.AnnouncementsPath = folder.Path);
        var store = host.Service<IAmAPublicationRecordStore>();

        //Act
        store.Save(id, PublicationRecordTestData.RecordWithEveryState());

        //Assert
        var path = Path.Combine(folder.Path, id.Value, "published.json");
        Assert.True(File.Exists(path));
        var json = File.ReadAllText(path);
        Assert.Contains("\"renderingHash\"", json);
        Assert.Contains("\"state\"", json);
        Assert.Contains("\"mastodon\"", json);
        Assert.Contains("\n  ", json);
    }
}
