using System.Net;
using Microsoft.Extensions.Time.Testing;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.Retries;

public class ServerErrorCreateTests
{
    [Theory]
    [InlineData("x")]
    [InlineData("writeas")]
    public async Task When_creating_a_post_gets_a_server_error_should_return_indeterminate_without_retrying(string channel)
    {
        //Arrange
        using var host = RetryTestData.HostWith(new FakeTimeProvider());
        host.Http.RespondWith(HttpStatusCode.BadGateway, """{"detail":"Bad gateway","error_msg":"Bad gateway"}""");

        //Act
        var result = await RetryTestData.CreateAsync(host, channel);

        //Assert
        Assert.IsType<PostResult.Indeterminate>(result);
        Assert.Single(host.Http.Requests);
    }
}
