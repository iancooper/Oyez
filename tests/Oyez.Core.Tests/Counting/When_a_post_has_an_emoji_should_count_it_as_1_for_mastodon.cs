using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Counting;

public class EmojiCountingForMastodonTests : OyezApplicationTest
{
    [Fact]
    public async Task When_a_post_has_an_emoji_should_count_it_as_1_for_mastodon()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("Mastodon", "Ship it 🚀"), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        var post = Assert.Single(Assert.Single(result.Value.Renderings).Posts);
        Assert.Equal("Ship it ".Length + 1, post.Count);
    }
}
