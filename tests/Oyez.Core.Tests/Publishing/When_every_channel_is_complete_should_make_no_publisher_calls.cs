using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class CompleteChannelTests : OyezApplicationTest
{
    [Fact]
    public async Task When_every_channel_is_complete_should_make_no_publisher_calls()
    {
        //Arrange
        Host.Announcements.Add(PublishingTestData.Announcement(Id, "x, mastodon", ["Released"]));
        var done = new ChannelPublication(ChannelState.Complete, new RenderingHash("sha256:earlier"), []);
        Host.PublicationRecords.Save(Id, PublishingTestData.RecordWith((ChannelId.X, done), (ChannelId.Mastodon, done)));

        //Act
        await Host.CommandProcessor.SendAsync(new PublishDueAnnouncements(), cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        Assert.Empty(Host.PublisherFor(ChannelId.X).Requests);
        Assert.Empty(Host.PublisherFor(ChannelId.Mastodon).Requests);
    }
}
