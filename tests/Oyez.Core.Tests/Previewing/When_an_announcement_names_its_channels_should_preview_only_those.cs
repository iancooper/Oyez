using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Previewing;

public class PreviewChannelSelectionTests : OyezApplicationTest
{
    [Fact]
    public async Task When_an_announcement_names_its_channels_should_preview_only_those()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(
            Id,
            """
            title: Release
            channels: x, WriteAs
            ---
            Segment
            """,
            WriteAsMarkdown: "Body"));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal([ChannelId.X, ChannelId.WriteAs], result.Value.Renderings.Select(r => r.Channel));
    }
}
