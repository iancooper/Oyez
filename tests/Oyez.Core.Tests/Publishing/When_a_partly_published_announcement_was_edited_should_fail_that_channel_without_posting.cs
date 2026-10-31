using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class EditedAfterPartialPublishTests : OyezApplicationTest
{
    [Fact]
    public async Task When_a_partly_published_announcement_was_edited_should_fail_that_channel_without_posting()
    {
        //Arrange
        var first = AnnouncementText.OfLength(250, 'a');
        Host.Announcements.Add(PublishingTestData.Announcement(Id, "x", [first, AnnouncementText.OfLength(250, 'b')]));
        var publisher = Host.PublisherFor(ChannelId.X);
        publisher.RespondFrom(index: 1, result: new PostResult.Rejected("rate limited"));
        await Host.CommandProcessor.SendAsync(new PublishDueAnnouncements(), cancellationToken: TestContext.Current.CancellationToken);
        publisher.RespondNormally();
        var attemptsBefore = publisher.Requests.Count;
        Host.Announcements.Add(PublishingTestData.Announcement(Id, "x", [first, AnnouncementText.OfLength(250, 'c')]));
        var command = new PublishDueAnnouncements();

        //Act
        await Host.CommandProcessor.SendAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        var channel = Host.PublicationRecords.Load(Id).Channels[ChannelId.X];
        Assert.Equal(attemptsBefore, publisher.Requests.Count);
        Assert.Equal(ChannelState.Failed, channel.State);
        Assert.Contains("resolve", channel.Reason, StringComparison.OrdinalIgnoreCase);
        Assert.Single(channel.Posts);
        Assert.Equal(ChannelState.Failed, Assert.Single(command.Result!.Outcomes).State);
    }
}
