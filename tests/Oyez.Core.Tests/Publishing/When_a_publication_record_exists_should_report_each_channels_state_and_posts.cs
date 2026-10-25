using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class PublishedStatusTests : OyezApplicationTest
{
    [Fact]
    public async Task When_a_publication_record_exists_should_report_each_channels_state_and_posts()
    {
        //Arrange
        var xPost = new PublishedPost(0, new PostRef("1847", "https://x.com/ICooper/status/1847"), new DateTimeOffset(2026, 10, 20, 9, 7, 12, TimeSpan.Zero));
        var x = new ChannelPublication(ChannelState.Complete, new RenderingHash("sha256:x"), [xPost]);
        var linkedIn = new ChannelPublication(ChannelState.Indeterminate, new RenderingHash("sha256:li"), [], "Timed out creating post 1");
        Host.PublicationRecords.Save(Id, new PublicationRecord(new Dictionary<ChannelId, ChannelPublication>
        {
            [ChannelId.X] = x,
            [ChannelId.LinkedIn] = linkedIn
        }));

        //Act
        var status = await Host.QueryProcessor.ExecuteAsync(new GetPublicationStatus(Id), TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal(2, status.Channels.Count);
        Assert.Equal(ChannelState.Complete, status.Channels[ChannelId.X].State);
        Assert.Equal(xPost, Assert.Single(status.Channels[ChannelId.X].Posts));
        Assert.Equal(ChannelState.Indeterminate, status.Channels[ChannelId.LinkedIn].State);
        Assert.Equal("Timed out creating post 1", status.Channels[ChannelId.LinkedIn].Reason);
    }
}
