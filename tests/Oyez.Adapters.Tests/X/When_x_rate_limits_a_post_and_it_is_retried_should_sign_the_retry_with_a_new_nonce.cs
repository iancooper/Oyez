namespace Oyez.Adapters.Tests.X;

public class RetryNonceTests
{
    [Fact]
    public async Task When_x_rate_limits_a_post_and_it_is_retried_should_sign_the_retry_with_a_new_nonce()
    {
        //Arrange
        var clock = new SchedulingClock();

        //Act
        var requests = await RateLimitedXPostTestData.PostRetriedAfterRateLimitAsync(clock);

        //Assert
        Assert.Equal(2, requests.Count);
        Assert.Equal("nonce1", RateLimitedXPostTestData.OAuthValue(requests[0], "oauth_nonce"));
        Assert.Equal("nonce2", RateLimitedXPostTestData.OAuthValue(requests[1], "oauth_nonce"));
    }
}
