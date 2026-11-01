using System.Net;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.WriteAs;

public class WriteAsRejectsPostTests
{
    [Fact]
    public async Task When_write_as_rejects_a_post_should_return_rejected_with_the_reason()
    {
        //Arrange
        using var host = new OyezAdaptersHost(WriteAsTestData.Configure);
        host.Http.RespondWith(HttpStatusCode.Unauthorized, """{"code":401,"error_msg":"Invalid access token."}""");

        //Act
        var result = await host.PublisherFor(ChannelId.WriteAs).CreateAsync(WriteAsTestData.Article(), CancellationToken.None);

        //Assert
        var rejected = Assert.IsType<PostResult.Rejected>(result);
        Assert.Contains("Invalid access token.", rejected.Reason);
    }
}
