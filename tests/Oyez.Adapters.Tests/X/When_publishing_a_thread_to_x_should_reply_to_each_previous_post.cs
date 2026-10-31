using System.Text.Json;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.X;

public class XThreadTests
{
    [Fact]
    public async Task When_publishing_a_thread_to_x_should_reply_to_each_previous_post()
    {
        //Arrange
        using var host = new OyezAdaptersHost(XTestData.Configure);
        host.Http.RespondInSequence("""{"data":{"id":"1001"}}""", """{"data":{"id":"1002"}}""", """{"data":{"id":"1003"}}""");
        var publisher = host.PublisherFor(ChannelId.X);

        //Act
        var first = Assert.IsType<PostResult.Posted>(await publisher.CreateAsync(XTestData.Request(0), CancellationToken.None));
        var second = Assert.IsType<PostResult.Posted>(await publisher.CreateAsync(XTestData.Request(1, first.Ref), CancellationToken.None));
        await publisher.CreateAsync(XTestData.Request(2, second.Ref), CancellationToken.None);

        //Assert
        Assert.Equal(3, host.Http.Requests.Count);
        Assert.All(host.Http.Requests, sent => Assert.Equal("https://api.x.com/2/tweets", sent.Uri.ToString()));
        var bodies = host.Http.Requests.Select(sent => JsonDocument.Parse(sent.Body).RootElement).ToList();
        Assert.False(bodies[0].TryGetProperty("reply", out _));
        Assert.Equal("1001", bodies[1].GetProperty("reply").GetProperty("in_reply_to_tweet_id").GetString());
        Assert.Equal("1002", bodies[2].GetProperty("reply").GetProperty("in_reply_to_tweet_id").GetString());
        Assert.Equal("It adds batching.", bodies[1].GetProperty("text").GetString());
    }
}
