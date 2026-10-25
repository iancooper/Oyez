using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class UntargetedChannelTests() : OyezApplicationTest(options => options.Mastodon.Enabled = false)
{
    [Fact]
    public async Task When_a_channel_is_not_targeted_or_not_enabled_should_not_publish_to_it()
    {
        //Arrange
        Host.Announcements.Add(PublishingTestData.Announcement(Id, "x, mastodon, linkedin", ["Released"]));

        //Act
        await Host.CommandProcessor.SendAsync(new PublishDueAnnouncements(), cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        Assert.Single(Host.PublisherFor(ChannelId.X).Requests);
        Assert.Single(Host.PublisherFor(ChannelId.LinkedIn).Requests);
        Assert.Empty(Host.PublisherFor(ChannelId.Mastodon).Requests);
        Assert.Empty(Host.PublisherFor(ChannelId.WriteAs).Requests);
    }
}
