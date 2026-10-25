using Oyez.Core.Announcements;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidateDuplicateHeaderKeyTests : OyezApplicationTest
{
    [Fact]
    public async Task When_a_header_key_appears_twice_should_report_it()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(
            Id,
            """
            title: Release
            Title: Another release
            ---
            Segment
            """,
            WriteAsMarkdown: null));

        //Act
        var errors = await Host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([Id]), TestContext.Current.CancellationToken);

        //Assert
        var error = Assert.Single(errors);
        Assert.Equal(Id, error.Announcement);
        Assert.Contains("Title", error.Message);
        Assert.Contains("Remove", error.Message);
    }
}
