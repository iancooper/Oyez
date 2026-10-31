using System.Net;
using Oyez.Core.Channels;

namespace Oyez.Adapters.Tests.LinkedIn;

public class LinkedInVerifyTests
{
    [Fact]
    public async Task When_verifying_linkedin_credentials_should_call_userinfo()
    {
        //Arrange
        using var host = new OyezAdaptersHost(LinkedInTestData.Configure, LinkedInTestData.FixTime);
        host.Http.RespondWith(HttpStatusCode.OK, """{"sub":"8675309","name":"Ian Cooper"}""");

        //Act
        var status = await host.PublisherFor(ChannelId.LinkedIn).VerifyAsync(CancellationToken.None);

        //Assert
        var sent = Assert.Single(host.Http.Requests);
        Assert.Equal(HttpMethod.Get, sent.Method);
        Assert.Equal("https://api.linkedin.com/v2/userinfo", sent.Uri.ToString());
        Assert.Equal("Bearer linkedin-token", sent.Headers["Authorization"]);
        Assert.True(status.IsValid);
        Assert.Equal(LinkedInTestData.NOW.AddDays(60), status.ExpiresAt);
    }
}
