using Oyez.Core.Announcements;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidateAnnouncementIdTests : OyezApplicationTest
{
    private readonly AnnouncementId _invalidId = new("Brighter_10.3.0");

    [Fact]
    public async Task When_an_announcement_id_is_not_a_valid_folder_name_should_report_it()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(
            _invalidId,
            """
            title: Release
            ---
            Segment
            """,
            WriteAsMarkdown: null));

        //Act
        var errors = await Host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([_invalidId]), TestContext.Current.CancellationToken);

        //Assert
        var error = Assert.Single(errors);
        Assert.Equal(_invalidId, error.Announcement);
        Assert.Contains("Brighter_10.3.0", error.Message);
    }
}
