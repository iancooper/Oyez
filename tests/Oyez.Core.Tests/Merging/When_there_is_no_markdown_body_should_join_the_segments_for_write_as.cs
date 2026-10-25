using Oyez.Core.Announcements;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Merging;

public class WriteAsWithoutMarkdownTests : OyezApplicationTest
{
    [Fact]
    public async Task When_there_is_no_markdown_body_should_join_the_segments_for_write_as()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("writeas", "First segment", "Second segment"), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        var rendering = Assert.Single(result.Value.Renderings);
        Assert.Equal("Release", rendering.Title);
        Assert.Equal("First segment\n\nSecond segment", Assert.Single(rendering.Posts).Text);
    }
}
