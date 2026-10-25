using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class ThreadChainingTests : OyezApplicationTest
{
    [Fact]
    public async Task When_publishing_a_thread_should_reply_to_the_previous_post()
    {
        //Arrange
        var firstPost = AnnouncementText.OfLength(250, 'a');
        var secondPost = AnnouncementText.OfLength(250, 'b');
        Host.Announcements.Add(PublishingTestData.Announcement(Id.Value, "x", null, firstPost, secondPost));

        //Act
        await Host.CommandProcessor.SendAsync(new PublishDueAnnouncements(), cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        var requests = Host.PublisherFor(ChannelId.X).Requests;
        var recorded = Host.PublicationRecords.Load(Id).Channels[ChannelId.X].Posts;
        Assert.Equal([0, 1], requests.Select(request => request.Index));
        Assert.Null(requests[0].ReplyTo);
        Assert.Equal(recorded[0].Ref, requests[1].ReplyTo);
    }
}
