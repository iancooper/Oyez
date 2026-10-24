using Oyez.Core.Announcements;
using Oyez.Core.Costing;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Costing;

public class XCostEstimateTests : IDisposable
{
    private const int LIMIT_FORCING_ONE_POST_PER_SEGMENT = 35;
    private const decimal POST_PRICE = 0.01m;
    private const decimal POST_WITH_URL_PRICE = 0.30m;

    private readonly OyezApplicationHost _host = new(options =>
    {
        options.X.Limit = LIMIT_FORCING_ONE_POST_PER_SEGMENT;
        options.X.Pricing.Post = POST_PRICE;
        options.X.Pricing.PostWithUrl = POST_WITH_URL_PRICE;
    });

    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_previewing_a_thread_should_estimate_its_x_cost()
    {
        //Arrange
        var postWithUrl = "Out now https://example.com/r";
        var plainPost = AnnouncementText.OfLength(30, 'b');
        _host.Announcements.Add(new AnnouncementDocument(_id, AnnouncementText.With("x", postWithUrl, plainPost, plainPost), WriteAsMarkdown: null));

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal(3, Assert.Single(result.Value.Renderings).Posts.Count);
        Assert.Equal(new UsDollars(POST_WITH_URL_PRICE + 2 * POST_PRICE), result.Value.EstimatedXCost);
    }

    public void Dispose() => _host.Dispose();
}
