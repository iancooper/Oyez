using Oyez.Core.Announcements;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.PublicationRecords;

public class UnknownChannelRecordTests
{
    [Fact]
    public void When_a_record_names_an_unknown_channel_should_refuse_to_load_it()
    {
        //Arrange
        using var folder = new TemporaryFolder();
        var id = new AnnouncementId("2026-11-03-brighter-10-3-0");
        folder.WriteFile($"{id.Value}/published.json", """{"channels":{"bluesky":{"state":"complete","renderingHash":"sha256:aaaa","posts":[],"reason":null}}}""");
        using var host = new OyezAdaptersHost(options => options.AnnouncementsPath = folder.Path);
        var store = host.Service<IAmAPublicationRecordStore>();

        //Act
        var failure = Record.Exception(() => store.Load(id));

        //Assert
        Assert.IsType<InvalidDataException>(failure);
    }
}
