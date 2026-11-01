using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.LinkedIn;

public class LinkedInTokenExpiredTests
{
    [Fact]
    public async Task When_the_linkedin_token_has_expired_should_return_rejected_without_calling_linkedin()
    {
        //Arrange
        var expiredYesterday = LinkedInTestData.NOW.AddDays(-1);
        using var host = new OyezAdaptersHost(LinkedInTestData.ConfigureExpiringAt(expiredYesterday), LinkedInTestData.FixTime);
        host.Http.RespondWith(_ => LinkedInTestData.CreatedWithId("urn:li:share:7251"));

        //Act
        var result = await host.PublisherFor(ChannelId.LinkedIn).CreateAsync(PostRequests.Build(ChannelId.LinkedIn, 3000, null, ["Brighter 10.1 is out. It adds batching."]), CancellationToken.None);

        //Assert
        var rejected = Assert.IsType<PostResult.Rejected>(result);
        Assert.Equal("LinkedIn token expired on 2026-10-30; run oyez auth linkedin", rejected.Reason);
        Assert.Empty(host.Http.Requests);
    }
}
