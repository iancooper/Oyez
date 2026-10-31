using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Counting;

public class XCjkCountingTests : OyezApplicationTest
{
    [Fact]
    public async Task When_a_segment_has_cjk_characters_should_count_each_as_2_for_x()
    {
        //Arrange
        var text = "日本語 abc";
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("X", text), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        var post = Assert.Single(Assert.Single(result.Value.Renderings).Posts);
        Assert.Equal(10, post.Count);
    }
}
