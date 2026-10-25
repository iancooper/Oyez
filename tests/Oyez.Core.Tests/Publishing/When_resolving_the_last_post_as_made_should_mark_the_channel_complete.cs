using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class ResolveLastPostAsMadeTests : OyezApplicationTest
{
    [Fact]
    public async Task When_resolving_the_last_post_as_made_should_mark_the_channel_complete()
    {
        //Arrange
        Host.Announcements.Add(PublishingTestData.Announcement(
            Id.Value, "x", null, AnnouncementText.OfLength(250, 'a'), AnnouncementText.OfLength(250, 'b')));
        var publisher = Host.PublisherFor(ChannelId.X);
        publisher.RespondFrom(index: 1, result: new PostResult.Indeterminate("timed out creating post 2"));
        await Host.CommandProcessor.SendAsync(new PublishDueAnnouncements(), cancellationToken: TestContext.Current.CancellationToken);
        var lastPost = new PostRef("x-checked-by-hand", null);
        var attemptsBefore = publisher.Requests.Count;

        //Act
        await Host.CommandProcessor.SendAsync(new ResolveChannel(Id, ChannelId.X, lastPost), cancellationToken: TestContext.Current.CancellationToken);
        await Host.CommandProcessor.SendAsync(new PublishDueAnnouncements(), cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        var publication = Host.PublicationRecords.Load(Id).Channels[ChannelId.X];
        Assert.Equal(ChannelState.Complete, publication.State);
        Assert.Null(publication.Reason);
        Assert.Equal(lastPost, publication.Posts[^1].Ref);
        Assert.Equal(attemptsBefore, publisher.Requests.Count);
    }
}
