using System.Net;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.LinkedIn;

public class LinkedInRejectsPostWithoutJsonTests
{
    [Fact]
    public async Task When_linkedin_rejects_a_post_without_a_json_body_should_still_return_rejected()
    {
        //Arrange
        using var host = new OyezAdaptersHost(LinkedInTestData.Configure, LinkedInTestData.FixTime);
        host.Http.RespondWith(HttpStatusCode.Unauthorized, "<html>Unauthorized</html>");

        //Act
        var result = await host.PublisherFor(ChannelId.LinkedIn).CreateAsync(LinkedInTestData.Request(), CancellationToken.None);

        //Assert
        var rejected = Assert.IsType<PostResult.Rejected>(result);
        Assert.Contains("401", rejected.Reason);
    }
}
