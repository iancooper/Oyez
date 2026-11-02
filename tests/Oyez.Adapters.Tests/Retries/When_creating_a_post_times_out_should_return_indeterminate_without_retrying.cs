using Microsoft.Extensions.Time.Testing;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.Retries;

public class TimedOutCreateTests
{
    [Theory]
    [MemberData(nameof(ChannelUnderTest.ALL), MemberType = typeof(ChannelUnderTest))]
    public async Task When_creating_a_post_times_out_should_return_indeterminate_without_retrying(ChannelUnderTest channel)
    {
        //Arrange
        using var host = RetryTestData.HostWith(new FakeTimeProvider());
        host.Http.Throw(new TaskCanceledException("The request timed out.", new TimeoutException()));

        //Act
        var result = await RetryTestData.CreateAsync(host, channel);

        //Assert
        Assert.IsType<PostResult.Indeterminate>(result);
        Assert.Single(host.Http.Requests);
    }
}
