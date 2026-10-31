using System.Net;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.WriteAs;

public class WriteAsAcceptsPostTests
{
    [Fact]
    public async Task When_write_as_accepts_a_post_should_return_its_url()
    {
        //Arrange
        using var host = new OyezAdaptersHost(WriteAsTestData.Configure);
        host.Http.RespondWith(HttpStatusCode.Created, """{"code":201,"data":{"id":"abc123","slug":"brighter-10-1"}}""");

        //Act
        var result = await host.PublisherFor(ChannelId.WriteAs).CreateAsync(WriteAsTestData.Request(), CancellationToken.None);

        //Assert
        var posted = Assert.IsType<PostResult.Posted>(result);
        Assert.Equal("abc123", posted.Ref.Id);
        Assert.Equal("https://ian-cooper.writeas.com/brighter-10-1", posted.Ref.Url);
    }
}
