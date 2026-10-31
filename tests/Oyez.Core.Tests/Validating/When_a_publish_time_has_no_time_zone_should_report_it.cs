using Oyez.Core.Announcements;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidatePublishTimeZoneTests : OyezApplicationTest
{
    [Fact]
    public async Task When_a_publish_time_has_no_time_zone_should_report_it()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(
            Id,
            """
            title: Release
            publish: 2026-11-03T09:00
            ---
            Segment
            """,
            WriteAsMarkdown: null));

        //Act
        var errors = await Host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([Id]), TestContext.Current.CancellationToken);

        //Assert
        var error = Assert.Single(errors);
        Assert.Equal(Id, error.Announcement);
        Assert.Contains("time zone", error.Message, StringComparison.OrdinalIgnoreCase);
    }
}
