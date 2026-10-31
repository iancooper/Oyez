using System.Net;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.X;

public class XAcceptsPostTests
{
    [Fact]
    public async Task When_x_accepts_a_post_should_return_its_url()
    {
        //Arrange
        using var host = new OyezAdaptersHost(XTestData.Configure);
        host.Http.RespondWith(HttpStatusCode.Created, """{"data":{"id":"1846987139428636021","text":"Brighter 10.1 is out."}}""");

        //Act
        var result = await host.PublisherFor(ChannelId.X).CreateAsync(XTestData.Request(), CancellationToken.None);

        //Assert
        var posted = Assert.IsType<PostResult.Posted>(result);
        Assert.Equal("1846987139428636021", posted.Ref.Id);
        Assert.Equal("https://x.com/ICooper/status/1846987139428636021", posted.Ref.Url);
    }
}
