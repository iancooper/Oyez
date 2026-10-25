using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class ChannelIndependenceTests : OyezApplicationTest
{
    [Fact]
    public async Task When_one_channel_is_rejected_should_still_publish_the_others()
    {
        //Arrange
        Host.Announcements.Add(PublishingTestData.Announcement(Id, "x, mastodon", ["Released"]));
        Host.PublisherFor(ChannelId.X).RespondFrom(index: 0, result: new PostResult.Rejected("account suspended"));
        var command = new PublishDueAnnouncements();

        //Act
        await Host.CommandProcessor.SendAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        var channels = Host.PublicationRecords.Load(Id).Channels;
        Assert.Equal(ChannelState.Failed, channels[ChannelId.X].State);
        Assert.Equal("account suspended", channels[ChannelId.X].Reason);
        Assert.Equal(ChannelState.Complete, channels[ChannelId.Mastodon].State);
        Assert.Equal(
            [(ChannelId.X, ChannelState.Failed), (ChannelId.Mastodon, ChannelState.Complete)],
            command.Result!.Outcomes.Select(outcome => (outcome.Channel, outcome.State)));
    }
}
