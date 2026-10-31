using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Counting;

public class XKeycapEmojiCountingTests : OyezApplicationTest
{
    [Theory]
    [InlineData("1⃣")]
    [InlineData("1️⃣")]
    [InlineData("#️⃣")]
    [InlineData("*️⃣")]
    public async Task When_a_segment_has_a_keycap_emoji_should_count_it_as_2_for_x(string keycap)
    {
        //Arrange
        var text = $"{keycap} abcdefghi";
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("X", text), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        var post = Assert.Single(Assert.Single(result.Value.Renderings).Posts);
        Assert.Equal(12, post.Count);
    }
}
