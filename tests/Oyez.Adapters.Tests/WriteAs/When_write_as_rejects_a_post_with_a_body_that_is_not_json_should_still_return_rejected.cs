using System.Net;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.WriteAs;

public class WriteAsRejectsPostWithoutJsonTests
{
    [Fact]
    public async Task When_write_as_rejects_a_post_with_a_body_that_is_not_json_should_still_return_rejected()
    {
        //Arrange
        using var host = new OyezAdaptersHost(WriteAsTestData.Configure);
        host.Http.RespondWith(HttpStatusCode.Unauthorized, "<html>Unauthorized</html>");
        PostResult? result = null;
        Exception? thrown = null;

        //Act
        try
        {
            result = await host.PublisherFor(ChannelId.WriteAs).CreateAsync(WriteAsTestData.Article(), CancellationToken.None);
        }
        catch (Exception exception)
        {
            thrown = exception;
        }

        //Assert
        Assert.Null(thrown);
        var rejected = Assert.IsType<PostResult.Rejected>(result);
        Assert.Contains("401", rejected.Reason);
    }
}
