using Oyez.Core.Announcements;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Previewing;

public class PreviewHeaderKeyTests : OyezApplicationTest
{
    [Fact]
    public async Task When_header_keys_vary_in_case_or_comments_are_present_should_still_read_the_header()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(
            Id,
            """
            # A comment explaining the release

            Title: Release
            PUBLISH: 2026-11-03T09:00Z
            ---
            Segment
            """,
            WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal("Release", result.Value.Title);
        Assert.Equal(new DateTimeOffset(2026, 11, 3, 9, 0, 0, TimeSpan.Zero), result.Value.PublishAt);
    }
}
