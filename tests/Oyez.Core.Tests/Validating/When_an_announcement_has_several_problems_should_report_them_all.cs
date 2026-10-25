using Oyez.Core.Announcements;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidateSeveralProblemsTests : OyezApplicationTest
{
    [Fact]
    public async Task When_an_announcement_has_several_problems_should_report_them_all()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(
            Id,
            """
            tags: release
            channels: bluesky
            ---
            One
            ---

            """,
            WriteAsMarkdown: null));

        //Act
        var errors = await Host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([Id]), TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal(4, errors.Count);
        Assert.All(errors, error => Assert.Equal(Id, error.Announcement));
        Assert.Contains(errors, error => error.Message.Contains("title", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(errors, error => error.Message.Contains("tags"));
        Assert.Contains(errors, error => error.Message.Contains("bluesky"));
        Assert.Contains(errors, error => error.Segment == 2);
    }
}
