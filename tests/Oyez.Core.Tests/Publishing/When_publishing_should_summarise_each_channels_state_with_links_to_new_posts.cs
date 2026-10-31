using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class JobSummaryTests : OyezApplicationTest
{
    [Fact]
    public async Task When_publishing_should_summarise_each_channels_state_with_links_to_new_posts()
    {
        //Arrange
        Host.Announcements.Add(PublishingTestData.Announcement(Id, "x, mastodon", ["Released"]));
        Host.PublisherFor(ChannelId.X).RespondFrom(index: 0, result: new PostResult.Rejected("account suspended"));
        var command = new PublishDueAnnouncements();

        //Act
        await Host.CommandProcessor.SendAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        var mastodonUrl = Host.PublicationRecords.Load(Id).Channels[ChannelId.Mastodon].Posts[0].Ref.Url;
        var lines = command.Result!.JobSummary.Split('\n');
        var xLine = Assert.Single(lines, line => line.Contains("X") && line.Contains(nameof(ChannelState.Failed)));
        Assert.Contains("account suspended", xLine);
        var mastodonLine = Assert.Single(lines, line => line.Contains("Mastodon") && line.Contains(nameof(ChannelState.Complete)));
        Assert.Contains(mastodonUrl!, mastodonLine);
    }
}
