using System.Net.Sockets;
using Microsoft.Extensions.Time.Testing;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.Retries;

public class ConnectionResetCreateTests
{
    [Theory]
    [MemberData(nameof(ChannelUnderTest.ALL), MemberType = typeof(ChannelUnderTest))]
    public async Task When_the_connection_fails_after_the_request_may_have_been_sent_should_return_indeterminate(ChannelUnderTest channel)
    {
        //Arrange
        using var host = RetryTestData.HostWith(new FakeTimeProvider());
        host.Http.Throw(new HttpRequestException("Connection reset", new SocketException((int)SocketError.ConnectionReset)));
        PostResult? result = null;
        Exception? thrown = null;

        //Act
        try
        {
            result = await RetryTestData.CreateAsync(host, channel);
        }
        catch (Exception exception)
        {
            thrown = exception;
        }

        //Assert
        Assert.Null(thrown);
        var indeterminate = Assert.IsType<PostResult.Indeterminate>(result);
        Assert.Contains("may have been made", indeterminate.Reason);
        Assert.Single(host.Http.Requests);
    }
}
