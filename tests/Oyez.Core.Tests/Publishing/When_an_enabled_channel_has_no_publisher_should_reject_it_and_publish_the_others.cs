using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class MissingPublisherPublishTests() : OyezApplicationTest(channelsWithoutPublisher: [ChannelId.Mastodon])
{
    [Fact]
    public async Task When_an_enabled_channel_has_no_publisher_should_reject_it_and_publish_the_others()
    {
        //Arrange
        Host.Announcements.Add(PublishingTestData.Announcement(Id, "x, mastodon", ["Released"]));
        var command = new PublishDueAnnouncements();

        //Act
        await Host.CommandProcessor.SendAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        var channels = Host.PublicationRecords.Load(Id).Channels;
        Assert.Equal(ChannelState.Failed, channels[ChannelId.Mastodon].State);
        Assert.Equal("No publisher is configured for Mastodon.", channels[ChannelId.Mastodon].Reason);
        Assert.Equal(ChannelState.Complete, channels[ChannelId.X].State);
        Assert.Equal(
            [(ChannelId.X, ChannelState.Complete, (string?)null), (ChannelId.Mastodon, ChannelState.Failed, "No publisher is configured for Mastodon.")],
            command.Result!.Outcomes.Select(outcome => (outcome.Channel, outcome.State, outcome.Reason)));
    }
}
