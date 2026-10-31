using Oyez.Core.Announcements;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Merging;

public class MergeOverTheLimitTests() : OyezApplicationTest(options => options.Mastodon.Limit = 401)
{
    [Fact]
    public async Task When_a_merge_is_one_over_the_limit_should_not_merge()
    {
        //Arrange
        var segments = new[] { AnnouncementText.OfLength(200, 'a'), AnnouncementText.OfLength(200, 'b') };
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("mastodon", segments), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal([200, 200], Assert.Single(result.Value.Renderings).Posts.Select(p => p.Count));
    }
}
