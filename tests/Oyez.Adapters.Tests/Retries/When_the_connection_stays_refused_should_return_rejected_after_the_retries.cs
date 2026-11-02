using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.Retries;

public class ConnectionStaysRefusedTests
{
    [Theory]
    [MemberData(nameof(ChannelUnderTest.ALL), MemberType = typeof(ChannelUnderTest))]
    public async Task When_the_connection_stays_refused_should_return_rejected_after_the_retries(ChannelUnderTest channel)
    {
        //Arrange
        var clock = new SchedulingClock();
        using var host = RetryTestData.HostWith(clock);
        host.Http.Throw(RetryTestData.ConnectionRefused());
        PostResult? result = null;
        Exception? thrown = null;

        //Act
        var pending = RetryTestData.CreateAsync(host, channel);
        await clock.TimerCreatedAsync(1);
        clock.Advance(TimeSpan.FromSeconds(1));
        await clock.TimerCreatedAsync(2);
        clock.Advance(TimeSpan.FromSeconds(2));
        await clock.TimerCreatedAsync(3);
        clock.Advance(TimeSpan.FromSeconds(4));
        try
        {
            result = await pending;
        }
        catch (Exception exception)
        {
            thrown = exception;
        }

        //Assert
        Assert.Null(thrown);
        var rejected = Assert.IsType<PostResult.Rejected>(result);
        Assert.Contains("could not be reached", rejected.Reason);
        Assert.Equal(4, host.Http.Requests.Count);
    }
}
