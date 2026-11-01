using Microsoft.Extensions.Time.Testing;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.Retries;

public class ConnectionRefusedCreateTests
{
    [Theory]
    [MemberData(nameof(ChannelUnderTest.ALL), MemberType = typeof(ChannelUnderTest))]
    public async Task When_the_connection_is_refused_should_retry_the_post(ChannelUnderTest channel)
    {
        //Arrange
        var clock = new FakeTimeProvider();
        using var host = RetryTestData.HostWith(clock);
        host.Http.RespondWith(requestNumber => requestNumber == 1
            ? throw RetryTestData.ConnectionRefused()
            : RetryTestData.Respond(System.Net.HttpStatusCode.OK, channel.AcceptedBody));

        //Act
        var result = await RetryTestData.CompleteWithClockRunningAsync(RetryTestData.CreateAsync(host, channel), clock);

        //Assert
        Assert.IsType<PostResult.Posted>(result);
        Assert.Equal(2, host.Http.Requests.Count);
    }
}
