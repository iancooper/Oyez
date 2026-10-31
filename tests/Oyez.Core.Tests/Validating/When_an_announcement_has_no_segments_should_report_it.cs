using Oyez.Core.Announcements;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidateNoSegmentsTests : OyezApplicationTest
{
    [Fact]
    public async Task When_an_announcement_has_no_segments_should_report_it()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(
            Id,
            """
            title: Release
            """,
            WriteAsMarkdown: null));

        //Act
        var errors = await Host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([Id]), TestContext.Current.CancellationToken);

        //Assert
        var error = Assert.Single(errors);
        Assert.Equal(Id, error.Announcement);
        Assert.Contains("segment", error.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Null(error.Segment);
    }
}
