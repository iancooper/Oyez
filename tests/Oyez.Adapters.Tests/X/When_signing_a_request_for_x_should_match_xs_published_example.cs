using System.Net;
using Oyez.Core.Channels;

namespace Oyez.Adapters.Tests.X;

public class XSignatureTests
{
    private const string EXPECTED_AUTHORIZATION =
        "OAuth oauth_consumer_key=\"xvz1evFS4wEEPTGEFPHBog\", " +
        "oauth_nonce=\"kYjzVBB8Y0ZFabxSWbWovY3uYSQ2pTgmZeNu2VS4cg\", " +
        "oauth_signature=\"lr%2BtV%2FDKclEvXKVjG6tgaSSLV0k%3D\", " +
        "oauth_signature_method=\"HMAC-SHA1\", " +
        "oauth_timestamp=\"1318622958\", " +
        "oauth_token=\"370773112-GmHxMAgYyLbNEtIKZeRNFsMKPR9EyMZeS9weJAEb\", " +
        "oauth_version=\"1.0\"";

    [Fact]
    public async Task When_signing_a_request_for_x_should_match_xs_published_example()
    {
        //Arrange
        using var host = new OyezAdaptersHost(XTestData.ConfigureWithPublishedExampleCredentials, XTestData.FixPublishedExampleTimeAndNonce);
        host.Http.RespondWith(HttpStatusCode.Created, """{"data":{"id":"1001"}}""");

        //Act
        await host.PublisherFor(ChannelId.X).CreateAsync(XTestData.Request(), CancellationToken.None);

        //Assert
        var sent = Assert.Single(host.Http.Requests);
        Assert.Equal(EXPECTED_AUTHORIZATION, sent.Headers.GetValueOrDefault("Authorization"));
    }
}
