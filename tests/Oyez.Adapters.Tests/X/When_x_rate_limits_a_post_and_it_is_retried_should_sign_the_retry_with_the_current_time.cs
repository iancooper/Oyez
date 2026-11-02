namespace Oyez.Adapters.Tests.X;

public class RetryTimestampTests
{
    [Fact]
    public async Task When_x_rate_limits_a_post_and_it_is_retried_should_sign_the_retry_with_the_current_time()
    {
        //Arrange
        var clock = new SchedulingClock();
        var start = RateLimitedXPostTestData.START.ToUnixTimeSeconds();

        //Act
        var requests = await RateLimitedXPostTestData.PostRetriedAfterRateLimitAsync(clock);

        //Assert
        Assert.Equal(2, requests.Count);
        Assert.Equal(start.ToString(), RateLimitedXPostTestData.OAuthValue(requests[0], "oauth_timestamp"));
        Assert.Equal((start + 30).ToString(), RateLimitedXPostTestData.OAuthValue(requests[1], "oauth_timestamp"));
    }
}
