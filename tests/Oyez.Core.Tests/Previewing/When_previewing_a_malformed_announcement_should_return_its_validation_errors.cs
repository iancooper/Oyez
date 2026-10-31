using Oyez.Core.Announcements;
using Oyez.Core.Previewing;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Previewing;

public class PreviewMalformedAnnouncementTests : OyezApplicationTest
{
    [Fact]
    public async Task When_previewing_a_malformed_announcement_should_return_its_validation_errors()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(
            Id,
            """
            tags: release
            ---
            Segment
            """,
            WriteAsMarkdown: null));
        var validationErrors = await Host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([Id]), TestContext.Current.CancellationToken);

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
        Assert.Equal(validationErrors, result.Errors);
    }
}
