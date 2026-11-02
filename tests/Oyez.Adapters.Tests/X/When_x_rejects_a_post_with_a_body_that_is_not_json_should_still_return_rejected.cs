using System.Net;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.X;

public class XRejectsPostWithoutJsonTests
{
    [Fact]
    public async Task When_x_rejects_a_post_with_a_body_that_is_not_json_should_still_return_rejected()
    {
        //Arrange
        using var host = new OyezAdaptersHost(XTestData.Configure);
        host.Http.RespondWith(HttpStatusCode.Forbidden, "<html>Forbidden</html>");
        PostResult? result = null;
        Exception? thrown = null;

        //Act
        try
        {
            result = await host.PublisherFor(ChannelId.X).CreateAsync(PostRequests.Thread(ChannelId.X, 280), CancellationToken.None);
        }
        catch (Exception exception)
        {
            thrown = exception;
        }

        //Assert
        Assert.Null(thrown);
        var rejected = Assert.IsType<PostResult.Rejected>(result);
        Assert.Contains("403", rejected.Reason);
    }
}
