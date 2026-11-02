using System.Text.Json;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.Mastodon;

public class MastodonThreadTests
{
    [Fact]
    public async Task When_publishing_a_thread_to_mastodon_should_reply_to_the_previous_post_publicly()
    {
        //Arrange
        using var host = new OyezAdaptersHost(MastodonTestData.Configure);
        host.Http.RespondInSequence(
            """{"id":"1001","url":"https://hachyderm.io/@ian/1001"}""",
            """{"id":"1002","url":"https://hachyderm.io/@ian/1002"}""",
            """{"id":"1003","url":"https://hachyderm.io/@ian/1003"}""");
        var publisher = host.PublisherFor(ChannelId.Mastodon);

        //Act
        var first = Assert.IsType<PostResult.Posted>(await publisher.CreateAsync(PostRequests.Thread(ChannelId.Mastodon, 500, 0), CancellationToken.None));
        var second = Assert.IsType<PostResult.Posted>(await publisher.CreateAsync(PostRequests.Thread(ChannelId.Mastodon, 500, 1, first.Ref), CancellationToken.None));
        await publisher.CreateAsync(PostRequests.Thread(ChannelId.Mastodon, 500, 2, second.Ref), CancellationToken.None);

        //Assert
        Assert.Equal("https://hachyderm.io/@ian/1001", first.Ref.Url);
        Assert.Equal(3, host.Http.Requests.Count);
        Assert.All(host.Http.Requests, sent => Assert.Equal("https://hachyderm.io/api/v1/statuses", sent.Uri.ToString()));
        Assert.All(host.Http.Requests, sent => Assert.Equal("Bearer mastodon-token", sent.Headers["Authorization"]));
        var bodies = host.Http.Requests.Select(sent => JsonDocument.Parse(sent.Body).RootElement).ToList();
        Assert.All(bodies, body => Assert.Equal("public", body.GetProperty("visibility").GetString()));
        Assert.False(bodies[0].TryGetProperty("in_reply_to_id", out _));
        Assert.Equal("1001", bodies[1].GetProperty("in_reply_to_id").GetString());
        Assert.Equal("1002", bodies[2].GetProperty("in_reply_to_id").GetString());
        Assert.Equal("It adds batching.", bodies[1].GetProperty("status").GetString());
    }
}
