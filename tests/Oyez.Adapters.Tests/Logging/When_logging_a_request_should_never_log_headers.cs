using System.Net;
using Oyez.Core.Channels;
using Oyez.Adapters.Tests.WriteAs;

namespace Oyez.Adapters.Tests.Logging;

public class LoggingRequestTests
{
    [Fact]
    public async Task When_logging_a_request_should_never_log_headers()
    {
        //Arrange
        var logs = new CollectedLogs();
        using var host = new OyezAdaptersHost(WriteAsTestData.Configure, logs.CaptureIn);
        host.Http.RespondWith(HttpStatusCode.Created, """{"code":201,"data":{"id":"abc123","slug":"brighter-10-1"}}""");

        //Act
        await host.PublisherFor(ChannelId.WriteAs).CreateAsync(WriteAsTestData.Request(), CancellationToken.None);

        //Assert
        var request = Assert.Single(logs.Entries, entry => entry.Message.Contains("/api/collections/ian-cooper/posts"));
        Assert.Contains("POST", request.Message);
        Assert.Contains("write.as", request.Message);
        Assert.Contains("201", request.Message);
        Assert.DoesNotContain(logs.Entries, entry => entry.Message.Contains("writeas-secret") || entry.Message.Contains("Authorization"));
    }
}
