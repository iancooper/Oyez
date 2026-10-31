using System.Net;
using Oyez.Core.Channels;

namespace Oyez.Adapters.Tests.WriteAs;

public class VerifyWriteAsCredentialsTests
{
    [Fact]
    public async Task When_verifying_write_as_credentials_should_call_me()
    {
        //Arrange
        using var host = new OyezAdaptersHost(WriteAsTestData.Configure);
        host.Http.RespondWith(HttpStatusCode.OK, """{"code":200,"data":{"username":"ian-cooper"}}""");

        //Act
        var status = await host.PublisherFor(ChannelId.WriteAs).VerifyAsync(CancellationToken.None);

        //Assert
        var sent = Assert.Single(host.Http.Requests);
        Assert.Equal(HttpMethod.Get, sent.Method);
        Assert.Equal("https://write.as/api/me", sent.Uri.ToString());
        Assert.Equal("Token writeas-secret", sent.Headers["Authorization"]);
        Assert.True(status.IsValid);
    }
}
