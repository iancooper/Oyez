using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class SkippedIndeterminateNewPostsTests : OyezApplicationTest
{
    [Fact]
    public async Task When_an_indeterminate_channel_is_skipped_should_report_no_new_posts()
    {
        //Arrange
        const string EARLIER_POST_URL = "https://x.example/post/x-1";
        Host.Announcements.Add(PublishingTestData.Announcement(Id, "x", [AnnouncementText.OfLength(250, 'a'), AnnouncementText.OfLength(250, 'b')]));
        var posts = new[] { new PublishedPost(0, new PostRef("x-1", EARLIER_POST_URL), DateTimeOffset.UnixEpoch) };
        var indeterminate = new ChannelPublication(ChannelState.Indeterminate, new RenderingHash("sha256:earlier"), posts, "timed out creating post 2");
        Host.PublicationRecords.Save(Id, PublishingTestData.RecordWith((ChannelId.X, indeterminate)));
        var command = new PublishDueAnnouncements();

        //Act
        await Host.CommandProcessor.SendAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        var outcome = Assert.Single(command.Result!.Outcomes);
        Assert.Empty(outcome.NewPosts);
        Assert.DoesNotContain(EARLIER_POST_URL, command.Result.JobSummary);
    }
}
