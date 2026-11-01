using System.Net;
using Microsoft.Extensions.Time.Testing;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.Retries;

public class BriefRateLimitCreateTests
{
    [Theory]
    [InlineData("x")]
    [InlineData("writeas")]
    public async Task When_rate_limited_briefly_should_wait_and_retry(string channel)
    {
        //Arrange
        var clock = new FakeTimeProvider();
        using var host = RetryTestData.HostWith(clock);
        host.Http.RespondWith(requestNumber => requestNumber == 1
            ? RetryTestData.RateLimited(retryAfterSeconds: 30)
            : RetryTestData.Respond(HttpStatusCode.OK, RetryTestData.AcceptedBodyFor(channel)));

        //Act
        var pending = RetryTestData.CreateAsync(host, channel);
        await RetryTestData.WaitForRequestsAsync(host, 1);
        clock.Advance(TimeSpan.FromSeconds(29));
        await Task.Delay(50, TestContext.Current.CancellationToken);
        var retriedTooSoon = host.Http.Requests.Count > 1;
        clock.Advance(TimeSpan.FromSeconds(1));
        var result = await pending;

        //Assert
        Assert.False(retriedTooSoon);
        Assert.IsType<PostResult.Posted>(result);
        Assert.Equal(2, host.Http.Requests.Count);
    }
}
