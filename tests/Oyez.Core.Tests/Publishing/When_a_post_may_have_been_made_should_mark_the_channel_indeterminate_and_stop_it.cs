using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class MarkChannelIndeterminateTests : OyezApplicationTest
{
    [Fact]
    public async Task When_a_post_may_have_been_made_should_mark_the_channel_indeterminate_and_stop_it()
    {
        //Arrange
        Host.Announcements.Add(PublishingTestData.Announcement(
            Id.Value, "x, mastodon", null, AnnouncementText.OfLength(250, 'a'), AnnouncementText.OfLength(250, 'b'), AnnouncementText.OfLength(250, 'c')));
        Host.PublisherFor(ChannelId.X).RespondFrom(index: 1, result: new PostResult.Indeterminate("timed out creating post 2"));
        var command = new PublishDueAnnouncements();

        //Act
        await Host.CommandProcessor.SendAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        var channels = Host.PublicationRecords.Load(Id).Channels;
        Assert.Equal(ChannelState.Indeterminate, channels[ChannelId.X].State);
        Assert.Equal("timed out creating post 2", channels[ChannelId.X].Reason);
        Assert.Equal([0], channels[ChannelId.X].Posts.Select(post => post.Index));
        Assert.Equal(2, Host.PublisherFor(ChannelId.X).Requests.Count);
        Assert.Equal(ChannelState.Complete, channels[ChannelId.Mastodon].State);
        Assert.Equal(
            [(ChannelId.X, ChannelState.Indeterminate, "timed out creating post 2"), (ChannelId.Mastodon, ChannelState.Complete, null)],
            command.Result!.Outcomes.Select(outcome => (outcome.Channel, outcome.State, outcome.Reason)));
    }
}
