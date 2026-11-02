using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.LinkedIn;

public class LinkedInTokenExpiresNowTests
{
    [Fact]
    public async Task When_the_linkedin_token_expires_exactly_now_should_return_rejected_without_calling_linkedin()
    {
        //Arrange
        var expiresAt = LinkedInTestData.NOW;
        using var host = new OyezAdaptersHost(LinkedInTestData.ConfigureExpiringAt(expiresAt), LinkedInTestData.FixTime);
        host.Http.RespondWith(_ => LinkedInTestData.CreatedWithId("urn:li:share:7251"));

        //Act
        var result = await host.PublisherFor(ChannelId.LinkedIn).CreateAsync(PostRequests.Build(ChannelId.LinkedIn, 3000, null, ["Brighter 10.1 is out. It adds batching."]), CancellationToken.None);

        //Assert
        Assert.IsType<PostResult.Rejected>(result);
        Assert.Empty(host.Http.Requests);
    }
}
