using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Merging;

public class ShortSegmentsOnXTests : OyezApplicationTest
{
    [Fact]
    public async Task When_two_short_segments_fit_together_should_merge_them_for_x()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("x", "Brighter is out", "Thanks everyone"), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        var rendering = Assert.Single(result.Value.Renderings);
        Assert.Equal(ChannelId.X, rendering.Channel);
        Assert.Equal("Brighter is out\n\nThanks everyone", Assert.Single(rendering.Posts).Text);
    }
}
