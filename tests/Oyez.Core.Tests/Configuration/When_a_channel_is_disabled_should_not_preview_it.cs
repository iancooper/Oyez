using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Configuration;

public class DisabledChannelTests : IDisposable
{
    private readonly OyezApplicationHost _host = new(options => options.LinkedIn.Enabled = false);
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_a_channel_is_disabled_should_not_preview_it()
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(
            _id,
            """
            title: Release
            channels: x, linkedin
            ---
            Segment
            """,
            WriteAsMarkdown: null));

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal([ChannelId.X], result.Value.Renderings.Select(r => r.Channel));
    }

    public void Dispose() => _host.Dispose();
}
