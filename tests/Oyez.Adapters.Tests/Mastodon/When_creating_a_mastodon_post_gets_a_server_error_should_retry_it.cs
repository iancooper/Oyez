using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.Mastodon;

public class MastodonServerErrorTests
{
    [Fact]
    public async Task When_creating_a_mastodon_post_gets_a_server_error_should_retry_it()
    {
        //Arrange
        var clock = new SchedulingClock();
        using var host = new OyezAdaptersHost(MastodonTestData.Configure, services => services.AddSingleton<TimeProvider>(clock));
        host.Http.RespondWith(requestNumber => requestNumber < 3
            ? Responses.Respond(HttpStatusCode.BadGateway, """{"error":"Bad gateway"}""")
            : Responses.Respond(HttpStatusCode.OK, """{"id":"1001","url":"https://hachyderm.io/@ian/1001"}"""));

        //Act
        var pending = host.PublisherFor(ChannelId.Mastodon).CreateAsync(PostRequests.Thread(ChannelId.Mastodon, 500), CancellationToken.None);
        await clock.TimerCreatedAsync(1);
        clock.Advance(TimeSpan.FromSeconds(1));
        await clock.TimerCreatedAsync(2);
        clock.Advance(TimeSpan.FromSeconds(2));
        var result = await pending;

        //Assert
        Assert.IsType<PostResult.Posted>(result);
        Assert.Equal(3, host.Http.Requests.Count);
    }
}
