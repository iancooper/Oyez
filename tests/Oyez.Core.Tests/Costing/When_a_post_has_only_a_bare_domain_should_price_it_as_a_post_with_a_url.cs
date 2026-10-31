using Oyez.Core.Announcements;
using Oyez.Core.Costing;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Costing;

public class BareDomainCostTests() : OyezApplicationTest(options => options.X.Limit = LIMIT_FORCING_ONE_POST_PER_SEGMENT)
{
    private const int LIMIT_FORCING_ONE_POST_PER_SEGMENT = 35;

    [Fact]
    public async Task When_a_post_has_only_a_bare_domain_should_price_it_as_a_post_with_a_url()
    {
        //Arrange
        var postWithBareDomain = "Notes at example.com/r";
        var plainPost = AnnouncementText.OfLength(30, 'b');
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("x", postWithBareDomain, plainPost, plainPost), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal(3, Assert.Single(result.Value.Renderings).Posts.Count);
        Assert.Equal(new UsDollars(0.20m + 2 * 0.015m), result.Value.EstimatedXCost);
    }
}
