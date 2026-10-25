using Oyez.Core.Announcements;
using Oyez.Core.Costing;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Costing;

public class XCostEstimateTests() : OyezApplicationTest(options =>
    {
        options.X.Limit = LIMIT_FORCING_ONE_POST_PER_SEGMENT;
        options.X.Pricing.Post = POST_PRICE;
        options.X.Pricing.PostWithUrl = POST_WITH_URL_PRICE;
    })
{
    private const int LIMIT_FORCING_ONE_POST_PER_SEGMENT = 35;
    private const decimal POST_PRICE = 0.01m;
    private const decimal POST_WITH_URL_PRICE = 0.30m;

    [Fact]
    public async Task When_previewing_a_thread_should_estimate_its_x_cost()
    {
        //Arrange
        var postWithUrl = "Out now https://example.com/r";
        var plainPost = AnnouncementText.OfLength(30, 'b');
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("x", postWithUrl, plainPost, plainPost), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal(3, Assert.Single(result.Value.Renderings).Posts.Count);
        Assert.Equal(new UsDollars(POST_WITH_URL_PRICE + 2 * POST_PRICE), result.Value.EstimatedXCost);
    }
}
