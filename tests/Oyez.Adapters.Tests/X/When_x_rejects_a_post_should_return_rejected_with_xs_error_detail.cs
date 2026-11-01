using System.Net;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.X;

public class XRejectsPostTests
{
    [Fact]
    public async Task When_x_rejects_a_post_should_return_rejected_with_xs_error_detail()
    {
        //Arrange
        using var host = new OyezAdaptersHost(XTestData.Configure);
        host.Http.RespondWith(HttpStatusCode.Forbidden, """{"title":"Forbidden","detail":"You are not permitted to create a Tweet with duplicate content.","type":"about:blank","status":403}""");

        //Act
        var result = await host.PublisherFor(ChannelId.X).CreateAsync(PostRequests.Thread(ChannelId.X, 280), CancellationToken.None);

        //Assert
        var rejected = Assert.IsType<PostResult.Rejected>(result);
        Assert.Contains("You are not permitted to create a Tweet with duplicate content.", rejected.Reason);
    }
}
