using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Counting;

public class XEmojiCountingTests : OyezApplicationTest
{
    [Fact]
    public async Task When_a_segment_has_an_emoji_and_10_ascii_characters_should_count_12_for_x()
    {
        //Arrange
        var text = "😀 abcdefghi";
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("X", text), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        var post = Assert.Single(Assert.Single(result.Value.Renderings).Posts);
        Assert.Equal(12, post.Count);
    }
}
