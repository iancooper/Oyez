using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Configuration;

public class ChannelLimitTests : OyezApplicationTest
{
    [Fact]
    public async Task When_previewing_should_show_each_channels_limit_from_configuration()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(
            Id,
            """
            title: Release
            ---
            Segment
            """,
            WriteAsMarkdown: "Body"));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        var limits = result.Value.Renderings.ToDictionary(r => r.Channel, r => r.Limit);
        Assert.Equal(
            new Dictionary<ChannelId, int?>
            {
                [ChannelId.X] = 280,
                [ChannelId.Mastodon] = 2263,
                [ChannelId.LinkedIn] = 3000,
                [ChannelId.WriteAs] = null
            },
            limits);
    }
}
