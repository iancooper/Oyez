using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class DueAnnouncementTests : OyezApplicationTest
{
    [Fact]
    public async Task When_publishing_should_publish_only_announcements_that_are_due()
    {
        //Arrange
        Host.Announcements.Add(PublishingTestData.Announcement(new AnnouncementId("past"), "x", ["Past"], publish: "2026-11-03T09:00Z"));
        Host.Announcements.Add(PublishingTestData.Announcement(new AnnouncementId("future"), "x", ["Future"], publish: "2026-11-04T09:00Z"));
        Host.Announcements.Add(PublishingTestData.Announcement(new AnnouncementId("undated"), "x", ["Undated"]));
        var command = new PublishDueAnnouncements();

        //Act
        await Host.CommandProcessor.SendAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        var published = Host.PublisherFor(ChannelId.X).Requests.Select(request => request.Announcement.Value);
        Assert.Equal(["past", "undated"], published.Order());
        var reported = command.Result!.Outcomes.Select(outcome => outcome.Announcement.Value);
        Assert.Equal(["past", "undated"], reported.Order());
    }
}
