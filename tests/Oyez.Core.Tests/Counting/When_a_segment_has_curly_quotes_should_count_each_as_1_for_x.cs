using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Counting;

public class XCurlyQuoteCountingTests : OyezApplicationTest
{
    [Fact]
    public async Task When_a_segment_has_curly_quotes_should_count_each_as_1_for_x()
    {
        //Arrange
        var text = "“Hello” it’s";
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("X", text), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        var post = Assert.Single(Assert.Single(result.Value.Renderings).Posts);
        Assert.Equal(12, post.Count);
    }
}
