using Oyez.Core.Announcements;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Merging;

public class GreedyMergeTests() : OyezApplicationTest(options => options.Mastodon.Limit = 450)
{
    [Fact]
    public async Task When_segments_do_not_fit_together_should_start_a_new_post()
    {
        //Arrange
        var first = AnnouncementText.OfLength(200, 'a');
        var second = AnnouncementText.OfLength(200, 'b');
        var third = AnnouncementText.OfLength(200, 'c');
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("mastodon", first, second, third), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        var posts = Assert.Single(result.Value.Renderings).Posts;
        Assert.Equal([$"{first}\n\n{second}", third], posts.Select(p => p.Text));
        Assert.Equal([402, 200], posts.Select(p => p.Count));
    }
}
