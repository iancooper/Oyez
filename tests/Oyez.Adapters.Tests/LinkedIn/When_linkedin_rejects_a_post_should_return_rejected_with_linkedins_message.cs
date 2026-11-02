using System.Net;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.LinkedIn;

public class LinkedInRejectsPostTests
{
    [Fact]
    public async Task When_linkedin_rejects_a_post_should_return_rejected_with_linkedins_message()
    {
        //Arrange
        using var host = new OyezAdaptersHost(LinkedInTestData.Configure, LinkedInTestData.FixTime);
        host.Http.RespondWith(HttpStatusCode.UnprocessableEntity, """{"status":422,"message":"Content is a duplicate of urn:li:share:7250"}""");

        //Act
        var result = await host.PublisherFor(ChannelId.LinkedIn).CreateAsync(PostRequests.Build(ChannelId.LinkedIn, 3000, null, ["Brighter 10.1 is out. It adds batching."]), CancellationToken.None);

        //Assert
        var rejected = Assert.IsType<PostResult.Rejected>(result);
        Assert.Contains("Content is a duplicate of urn:li:share:7250", rejected.Reason);
    }
}
