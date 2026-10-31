using System.Net;
using Oyez.Core.Channels;

namespace Oyez.Adapters.Tests.X;

public class VerifyXCredentialsTests
{
    [Fact]
    public async Task When_verifying_x_credentials_should_call_users_me()
    {
        //Arrange
        using var host = new OyezAdaptersHost(XTestData.Configure);
        host.Http.RespondWith(HttpStatusCode.OK, """{"data":{"id":"2244994945","name":"Ian Cooper","username":"ICooper"}}""");

        //Act
        var status = await host.PublisherFor(ChannelId.X).VerifyAsync(CancellationToken.None);

        //Assert
        var sent = Assert.Single(host.Http.Requests);
        Assert.Equal(HttpMethod.Get, sent.Method);
        Assert.Equal("https://api.x.com/2/users/me", sent.Uri.ToString());
        Assert.True(status.IsValid);
    }
}
