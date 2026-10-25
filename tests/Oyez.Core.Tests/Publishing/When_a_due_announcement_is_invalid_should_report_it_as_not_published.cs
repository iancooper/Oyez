using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class InvalidDueAnnouncementTests : OyezApplicationTest
{
    [Fact]
    public async Task When_a_due_announcement_is_invalid_should_report_it_as_not_published()
    {
        //Arrange
        var untitled = new AnnouncementId("untitled-and-due");
        var overlongFuture = new AnnouncementId("overlong-and-future");
        Host.Announcements.Add(new AnnouncementDocument(untitled, "channels: x\n---\nSegment", WriteAsMarkdown: null));
        Host.Announcements.Add(PublishingTestData.Announcement(overlongFuture, "x", [AnnouncementText.OfLength(300, 'a')], publish: "2026-11-04T09:00Z"));
        Host.Announcements.Add(PublishingTestData.Announcement(new AnnouncementId("valid"), "x", ["Valid"]));
        var command = new PublishDueAnnouncements();

        //Act
        await Host.CommandProcessor.SendAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        var report = command.Result!;
        var notPublished = Assert.Single(report.NotPublished);
        Assert.Equal(untitled, notPublished.Announcement);
        Assert.Contains("title", Assert.Single(notPublished.Errors).Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(["valid"], Host.PublisherFor(ChannelId.X).Requests.Select(request => request.Announcement.Value));
        Assert.Contains("untitled-and-due", report.JobSummary);
        Assert.DoesNotContain("overlong-and-future", report.JobSummary);
    }
}
