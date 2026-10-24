using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Configuration;

public class ConfiguredChannelLimitTests : IDisposable
{
    private readonly OyezApplicationHost _host = new(options => options.Mastodon.Limit = 500);
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_a_limit_is_configured_should_show_it_in_the_preview()
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(
            _id,
            """
            title: Release
            channels: mastodon
            ---
            Segment
            """,
            WriteAsMarkdown: null));

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal(500, Assert.Single(result.Value.Renderings).Limit);
    }

    public void Dispose() => _host.Dispose();
}
