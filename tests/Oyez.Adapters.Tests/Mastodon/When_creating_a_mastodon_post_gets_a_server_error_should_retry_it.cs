using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Oyez.Adapters.Tests.Retries;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.Mastodon;

public class MastodonServerErrorTests
{
    [Fact]
    public async Task When_creating_a_mastodon_post_gets_a_server_error_should_retry_it()
    {
        //Arrange
        var clock = new FakeTimeProvider();
        using var host = new OyezAdaptersHost(MastodonTestData.Configure, services => services.AddSingleton<TimeProvider>(clock));
        host.Http.RespondWith(request => request < 3
            ? RetryTestData.Respond(HttpStatusCode.BadGateway, """{"error":"Bad gateway"}""")
            : RetryTestData.Respond(HttpStatusCode.OK, """{"id":"1001","url":"https://hachyderm.io/@ian/1001"}"""));

        //Act
        var pending = host.PublisherFor(ChannelId.Mastodon).CreateAsync(MastodonTestData.Request(), CancellationToken.None);
        var result = await RetryTestData.CompleteWithClockRunningAsync(pending, clock);

        //Assert
        Assert.IsType<PostResult.Posted>(result);
        Assert.Equal(3, host.Http.Requests.Count);
    }
}
