using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class ResolveChannelErrorTests : OyezApplicationTest
{
    [Fact]
    public async Task When_resolving_a_channel_that_is_not_indeterminate_should_report_an_error()
    {
        //Arrange
        Host.Announcements.Add(PublishingTestData.Announcement(Id, "x", [AnnouncementText.OfLength(250, 'a'), AnnouncementText.OfLength(250, 'b')]));
        Host.PublisherFor(ChannelId.X).RespondFrom(index: 1, result: new PostResult.Rejected("rate limited"));
        await Host.CommandProcessor.SendAsync(new PublishDueAnnouncements(), cancellationToken: TestContext.Current.CancellationToken);
        var before = Host.PublicationRecords.Load(Id);
        var resolve = new ResolveChannel(Id, ChannelId.X, new PostRef("x-9", null));

        //Act
        await Host.CommandProcessor.SendAsync(resolve, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        Assert.False(resolve.Result!.IsSuccess);
        var error = Assert.Single(resolve.Result.Errors);
        Assert.Equal((Id, ChannelId.X), (error.Announcement, error.Channel));
        Assert.Contains("indeterminate", error.Message);
        Assert.Same(before, Host.PublicationRecords.Load(Id));
    }
}
