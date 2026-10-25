using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class ResolveChannelAsNotMadeTests : OyezApplicationTest
{
    [Fact]
    public async Task When_resolving_a_post_as_not_made_should_retry_it_on_the_next_run()
    {
        //Arrange
        Host.Announcements.Add(PublishingTestData.Announcement(
            Id.Value, "x", null, AnnouncementText.OfLength(250, 'a'), AnnouncementText.OfLength(250, 'b'), AnnouncementText.OfLength(250, 'c')));
        var publisher = Host.PublisherFor(ChannelId.X);
        publisher.IndeterminateFrom(index: 1, reason: "timed out creating post 2");
        await Host.CommandProcessor.SendAsync(new PublishDueAnnouncements(), cancellationToken: TestContext.Current.CancellationToken);
        publisher.StopBeingIndeterminate();
        var post1 = Host.PublicationRecords.Load(Id).Channels[ChannelId.X].Posts[0].Ref;
        var attemptsBefore = publisher.Requests.Count;
        var resolve = new ResolveChannel(Id, ChannelId.X, Posted: null);

        //Act
        await Host.CommandProcessor.SendAsync(resolve, cancellationToken: TestContext.Current.CancellationToken);
        await Host.CommandProcessor.SendAsync(new PublishDueAnnouncements(), cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        Assert.True(resolve.Result!.IsSuccess);
        var retried = publisher.Requests.Skip(attemptsBefore).ToList();
        Assert.Equal([1, 2], retried.Select(request => request.Index));
        Assert.Equal(post1, retried[0].ReplyTo);
        Assert.Equal(ChannelState.Complete, Host.PublicationRecords.Load(Id).Channels[ChannelId.X].State);
    }
}
