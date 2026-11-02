using System.Net;
using Oyez.Core.Channels;

namespace Oyez.Adapters.Tests.Mastodon;

public class VerifyMastodonCredentialsTests
{
    [Fact]
    public async Task When_verifying_mastodon_credentials_should_call_verify_credentials()
    {
        //Arrange
        using var host = new OyezAdaptersHost(MastodonTestData.Configure);
        host.Http.RespondWith(HttpStatusCode.OK, """{"id":"1","username":"ian","acct":"ian"}""");

        //Act
        var status = await host.PublisherFor(ChannelId.Mastodon).VerifyAsync(CancellationToken.None);

        //Assert
        var sent = Assert.Single(host.Http.Requests);
        Assert.Equal(HttpMethod.Get, sent.Method);
        Assert.Equal("https://hachyderm.io/api/v1/accounts/verify_credentials", sent.Uri.ToString());
        Assert.Equal("Bearer mastodon-token", sent.Headers["Authorization"]);
        Assert.True(status.IsValid);
    }
}
