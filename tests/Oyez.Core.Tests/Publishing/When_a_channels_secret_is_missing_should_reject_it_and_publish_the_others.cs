using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class MissingSecretTests : OyezApplicationTest
{
    [Fact]
    public async Task When_a_channels_secret_is_missing_should_reject_it_and_publish_the_others()
    {
        //Arrange
        Host.Announcements.Add(PublishingTestData.Announcement(Id.Value, "mastodon, x", null, "Released"));
        Host.PublisherFor(ChannelId.Mastodon).RejectFrom(index: 0, reason: "missing secret OYEZ_MASTODON_TOKEN");

        //Act
        await Host.CommandProcessor.SendAsync(new PublishDueAnnouncements(), cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        var channels = Host.PublicationRecords.Load(Id).Channels;
        Assert.Equal(ChannelState.Failed, channels[ChannelId.Mastodon].State);
        Assert.Equal("missing secret OYEZ_MASTODON_TOKEN", channels[ChannelId.Mastodon].Reason);
        Assert.Equal(ChannelState.Complete, channels[ChannelId.X].State);
    }
}
