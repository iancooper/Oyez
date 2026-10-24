using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Configuration;

public class ChannelLimitTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_previewing_should_show_each_channels_limit_from_configuration()
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(
            _id,
            """
            title: Release
            ---
            Segment
            """,
            WriteAsMarkdown: "Body"));

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

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

    public void Dispose() => _host.Dispose();
}
