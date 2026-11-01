using System.Net;
using Microsoft.Extensions.Time.Testing;

namespace Oyez.Adapters.Tests.Retries;

public class ReadServerErrorTests
{
    [Theory]
    [InlineData("x")]
    [InlineData("writeas")]
    public async Task When_a_read_gets_a_server_error_should_retry_with_backoff(string channel)
    {
        //Arrange
        var clock = new FakeTimeProvider();
        using var host = RetryTestData.HostWith(clock);
        host.Http.RespondWith(requestNumber => requestNumber < 3
            ? RetryTestData.Respond(HttpStatusCode.InternalServerError)
            : RetryTestData.Respond(HttpStatusCode.OK));

        //Act
        var pending = RetryTestData.VerifyAsync(host, channel);
        await RetryTestData.WaitForRequestsAsync(host, 1);
        clock.Advance(TimeSpan.FromSeconds(1));
        await RetryTestData.WaitForRequestsAsync(host, 2);
        clock.Advance(TimeSpan.FromSeconds(2));
        var status = await pending;

        //Assert
        Assert.True(status.IsValid);
        Assert.Equal(3, host.Http.Requests.Count);
    }
}
