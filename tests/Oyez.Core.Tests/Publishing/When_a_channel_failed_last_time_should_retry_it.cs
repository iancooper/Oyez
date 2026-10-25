using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class RetryFailedChannelTests : OyezApplicationTest
{
    [Fact]
    public async Task When_a_channel_failed_last_time_should_retry_it()
    {
        //Arrange
        Host.Announcements.Add(PublishingTestData.Announcement(Id, "x", ["Released"]));
        var failed = new ChannelPublication(ChannelState.Failed, new RenderingHash("sha256:earlier"), [], "account suspended");
        Host.PublicationRecords.Save(Id, PublishingTestData.RecordWith((ChannelId.X, failed)));

        //Act
        await Host.CommandProcessor.SendAsync(new PublishDueAnnouncements(), cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        var retried = Host.PublicationRecords.Load(Id).Channels[ChannelId.X];
        Assert.Single(Host.PublisherFor(ChannelId.X).Requests);
        Assert.Equal(ChannelState.Complete, retried.State);
        Assert.Null(retried.Reason);
    }
}
