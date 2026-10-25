using Oyez.Core.Announcements;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Previewing;

public class PreviewHeaderValueTests : OyezApplicationTest
{
    [Fact]
    public async Task When_a_header_value_contains_a_colon_should_keep_the_whole_value()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(
            Id,
            """
            title:   Brighter: the 10.3.0 release   
            ---
            Segment
            """,
            WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal("Brighter: the 10.3.0 release", result.Value.Title);
    }
}
