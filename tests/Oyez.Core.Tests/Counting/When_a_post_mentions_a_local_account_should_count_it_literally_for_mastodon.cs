using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Counting;

public class LocalMentionCountingForMastodonTests : OyezApplicationTest
{
    [Fact]
    public async Task When_a_post_mentions_a_local_account_should_count_it_literally_for_mastodon()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("Mastodon", "Thanks @someone"), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        var post = Assert.Single(Assert.Single(result.Value.Renderings).Posts);
        Assert.Equal("Thanks @someone".Length, post.Count);
    }
}
