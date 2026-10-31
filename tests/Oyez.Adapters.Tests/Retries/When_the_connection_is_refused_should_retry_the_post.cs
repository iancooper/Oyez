using Microsoft.Extensions.Time.Testing;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.Retries;

public class ConnectionRefusedCreateTests
{
    [Theory]
    [InlineData("x")]
    [InlineData("writeas")]
    public async Task When_the_connection_is_refused_should_retry_the_post(string channel)
    {
        //Arrange
        var clock = new FakeTimeProvider();
        using var host = RetryTestData.HostWith(clock);
        host.Http.RespondWith(request => request == 1
            ? throw RetryTestData.ConnectionRefused()
            : RetryTestData.Respond(System.Net.HttpStatusCode.OK, RetryTestData.AcceptedBodyFor(channel)));

        //Act
        var result = await RetryTestData.CompleteWithClockRunningAsync(RetryTestData.CreateAsync(host, channel), clock);

        //Assert
        Assert.IsType<PostResult.Posted>(result);
        Assert.Equal(2, host.Http.Requests.Count);
    }
}
