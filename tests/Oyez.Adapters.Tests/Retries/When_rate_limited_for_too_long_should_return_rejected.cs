using Microsoft.Extensions.Time.Testing;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.Retries;

public class LongRateLimitCreateTests
{
    [Theory]
    [MemberData(nameof(ChannelUnderTest.ALL), MemberType = typeof(ChannelUnderTest))]
    public async Task When_rate_limited_for_too_long_should_return_rejected(ChannelUnderTest channel)
    {
        //Arrange
        using var host = RetryTestData.HostWith(new FakeTimeProvider());
        host.Http.RespondWith(_ => RetryTestData.RateLimited(retryAfterSeconds: 120));

        //Act
        var result = await RetryTestData.CreateAsync(host, channel);

        //Assert
        var rejected = Assert.IsType<PostResult.Rejected>(result);
        Assert.Contains("rate limit", rejected.Reason, StringComparison.OrdinalIgnoreCase);
        Assert.Single(host.Http.Requests);
    }
}
