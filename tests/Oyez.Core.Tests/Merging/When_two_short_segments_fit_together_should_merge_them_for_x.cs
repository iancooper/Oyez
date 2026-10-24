using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Merging;

public class ShortSegmentsOnXTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_two_short_segments_fit_together_should_merge_them_for_x()
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(_id, AnnouncementText.With("x", "Brighter is out", "Thanks everyone"), WriteAsMarkdown: null));

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        var rendering = Assert.Single(result.Value.Renderings);
        Assert.Equal(ChannelId.X, rendering.Channel);
        Assert.Equal("Brighter is out\n\nThanks everyone", Assert.Single(rendering.Posts).Text);
    }

    public void Dispose() => _host.Dispose();
}
