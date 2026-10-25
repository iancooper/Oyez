using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class ResumeTests : OyezApplicationTest
{
    [Fact]
    public async Task When_some_posts_are_recorded_should_publish_only_the_rest()
    {
        //Arrange
        var posts = new[] { AnnouncementText.OfLength(250, 'a'), AnnouncementText.OfLength(250, 'b'), AnnouncementText.OfLength(250, 'c') };
        Host.Announcements.Add(PublishingTestData.Announcement(Id.Value, "x", null, posts));
        var publisher = Host.PublisherFor(ChannelId.X);
        publisher.RespondFrom(index: 1, result: new PostResult.Rejected("rate limited"));
        await Host.CommandProcessor.SendAsync(new PublishDueAnnouncements(), cancellationToken: TestContext.Current.CancellationToken);
        var recordedBefore = Host.PublicationRecords.Load(Id).Channels[ChannelId.X].Posts;
        publisher.RespondNormally();
        var attemptsBefore = publisher.Requests.Count;

        //Act
        await Host.CommandProcessor.SendAsync(new PublishDueAnnouncements(), cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        var resumed = publisher.Requests.Skip(attemptsBefore).ToList();
        Assert.Equal([1, 2], resumed.Select(request => request.Index));
        Assert.Equal(recordedBefore[0].Ref, resumed[0].ReplyTo);
        Assert.Equal(ChannelState.Complete, Host.PublicationRecords.Load(Id).Channels[ChannelId.X].State);
    }
}
