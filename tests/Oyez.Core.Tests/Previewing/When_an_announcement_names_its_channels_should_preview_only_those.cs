using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Previewing;

public class PreviewChannelSelectionTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_an_announcement_names_its_channels_should_preview_only_those()
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(
            _id,
            """
            title: Release
            channels: x, WriteAs
            ---
            Segment
            """,
            WriteAsMarkdown: "Body"));

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal([ChannelId.X, ChannelId.WriteAs], result.Value.Renderings.Select(r => r.Channel));
    }

    public void Dispose() => _host.Dispose();
}
