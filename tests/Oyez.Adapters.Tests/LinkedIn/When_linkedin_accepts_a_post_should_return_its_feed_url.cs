using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.LinkedIn;

public class LinkedInAcceptsPostTests
{
    [Fact]
    public async Task When_linkedin_accepts_a_post_should_return_its_feed_url()
    {
        //Arrange
        using var host = new OyezAdaptersHost(LinkedInTestData.Configure, LinkedInTestData.FixTime);
        host.Http.RespondWith(_ => LinkedInTestData.CreatedWithId("urn:li:share:7251"));

        //Act
        var result = await host.PublisherFor(ChannelId.LinkedIn).CreateAsync(PostRequests.Build(ChannelId.LinkedIn, 3000, null, ["Brighter 10.1 is out. It adds batching."]), CancellationToken.None);

        //Assert
        var posted = Assert.IsType<PostResult.Posted>(result);
        Assert.Equal("urn:li:share:7251", posted.Ref.Id);
        Assert.Equal("https://www.linkedin.com/feed/update/urn:li:share:7251", posted.Ref.Url);
    }
}
