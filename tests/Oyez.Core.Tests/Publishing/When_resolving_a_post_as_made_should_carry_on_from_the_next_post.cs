using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class ResolveChannelAsMadeTests : OyezApplicationTest
{
    [Fact]
    public async Task When_resolving_a_post_as_made_should_carry_on_from_the_next_post()
    {
        //Arrange
        Host.Announcements.Add(PublishingTestData.Announcement(
            Id.Value, "x", null, AnnouncementText.OfLength(250, 'a'), AnnouncementText.OfLength(250, 'b'), AnnouncementText.OfLength(250, 'c')));
        var publisher = Host.PublisherFor(ChannelId.X);
        publisher.RespondFrom(index: 1, result: new PostResult.Indeterminate("timed out creating post 2"));
        await Host.CommandProcessor.SendAsync(new PublishDueAnnouncements(), cancellationToken: TestContext.Current.CancellationToken);
        publisher.RespondNormally();
        var post2 = new PostRef("x-checked-by-hand", "https://x.example/checked-by-hand");
        var attemptsBefore = publisher.Requests.Count;
        var resolve = new ResolveChannel(Id, ChannelId.X, post2);

        //Act
        await Host.CommandProcessor.SendAsync(resolve, cancellationToken: TestContext.Current.CancellationToken);
        await Host.CommandProcessor.SendAsync(new PublishDueAnnouncements(), cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        Assert.True(resolve.Result!.IsSuccess);
        var resumed = Assert.Single(publisher.Requests.Skip(attemptsBefore));
        Assert.Equal(2, resumed.Index);
        Assert.Equal(post2, resumed.ReplyTo);
        var posts = Host.PublicationRecords.Load(Id).Channels[ChannelId.X].Posts;
        Assert.Equal([0, 1, 2], posts.Select(post => post.Index));
        Assert.Equal(post2, posts[1].Ref);
        Assert.Equal(Host.Clock.GetUtcNow(), posts[1].At);
    }
}
