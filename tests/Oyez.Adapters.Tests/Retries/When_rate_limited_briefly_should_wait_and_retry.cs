using System.Net;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.Retries;

public class BriefRateLimitCreateTests
{
    [Theory]
    [MemberData(nameof(ChannelUnderTest.ALL), MemberType = typeof(ChannelUnderTest))]
    public async Task When_rate_limited_briefly_should_wait_and_retry(ChannelUnderTest channel)
    {
        //Arrange
        var clock = new SchedulingClock();
        using var host = RetryTestData.HostWith(clock);
        host.Http.RespondWith(requestNumber => requestNumber == 1
            ? RetryTestData.RateLimited(retryAfterSeconds: 30)
            : Responses.Respond(HttpStatusCode.OK, channel.AcceptedBody));

        //Act
        var pending = RetryTestData.CreateAsync(host, channel);
        await host.Http.RequestArrivedAsync(1);
        await clock.TimerCreatedAsync(1);
        clock.Advance(TimeSpan.FromSeconds(29));
        var retriedTooSoon = host.Http.Requests.Count > 1;
        clock.Advance(TimeSpan.FromSeconds(1));
        var result = await pending;

        //Assert
        Assert.False(retriedTooSoon);
        Assert.IsType<PostResult.Posted>(result);
        Assert.Equal(2, host.Http.Requests.Count);
    }
}
