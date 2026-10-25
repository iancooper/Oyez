using Oyez.Core.Announcements;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Merging;

public class MergeOnTheLimitTests() : OyezApplicationTest(options => options.Mastodon.Limit = 402)
{
    [Fact]
    public async Task When_a_merge_lands_exactly_on_the_limit_should_merge()
    {
        //Arrange
        var segments = new[] { AnnouncementText.OfLength(200, 'a'), AnnouncementText.OfLength(200, 'b') };
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("mastodon", segments), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal(402, Assert.Single(Assert.Single(result.Value.Renderings).Posts).Count);
    }
}
