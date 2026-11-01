using System.Net;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.X;

public class XBodySignatureTests
{
    [Fact]
    public async Task When_signing_a_request_for_x_should_not_sign_the_json_body()
    {
        //Arrange
        using var host = new OyezAdaptersHost(XTestData.ConfigureWithPublishedExampleCredentials, XTestData.FixPublishedExampleTimeAndNonce);
        host.Http.RespondWith(HttpStatusCode.Created, """{"data":{"id":"1001"}}""");
        var publisher = host.PublisherFor(ChannelId.X);

        //Act
        await publisher.CreateAsync(PostRequests.Thread(ChannelId.X, 280, 0), CancellationToken.None);
        await publisher.CreateAsync(PostRequests.Thread(ChannelId.X, 280, 1, new PostRef("1001", null)), CancellationToken.None);

        //Assert
        var (first, second) = (host.Http.Requests[0], host.Http.Requests[1]);
        Assert.NotEqual(first.Body, second.Body);
        Assert.Equal(first.Headers["Authorization"], second.Headers["Authorization"]);
    }
}
