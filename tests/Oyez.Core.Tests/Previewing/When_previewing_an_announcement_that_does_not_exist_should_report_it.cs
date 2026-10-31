using Oyez.Core.Announcements;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Previewing;

public class PreviewMissingAnnouncementTests : OyezApplicationTest
{
    private readonly AnnouncementId _stored = new("2026-11-03-brighter-10-3-0");
    private readonly AnnouncementId _missing = new("2026-11-10-darker-3-1-0");

    [Fact]
    public async Task When_previewing_an_announcement_that_does_not_exist_should_report_it()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(_stored, "title: Release\n---\nSegment", WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_missing), TestContext.Current.CancellationToken);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(_missing, Assert.Single(result.Errors).Announcement);
    }
}
