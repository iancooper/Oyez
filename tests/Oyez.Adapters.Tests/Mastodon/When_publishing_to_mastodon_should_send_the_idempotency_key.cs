using Oyez.Core.Channels;

namespace Oyez.Adapters.Tests.Mastodon;

public class MastodonIdempotencyTests
{
    [Fact]
    public async Task When_publishing_to_mastodon_should_send_the_idempotency_key()
    {
        //Arrange
        using var host = new OyezAdaptersHost(MastodonTestData.Configure);
        host.Http.RespondInSequence("""{"id":"1001","url":"https://hachyderm.io/@ian/1001"}""");
        var request = PostRequests.Thread(ChannelId.Mastodon, 500);

        //Act
        await host.PublisherFor(ChannelId.Mastodon).CreateAsync(request, CancellationToken.None);

        //Assert
        var sent = Assert.Single(host.Http.Requests);
        sent.Headers.TryGetValue("Idempotency-Key", out var key);
        Assert.Equal("sha256:0123456789ab", key);
    }
}
