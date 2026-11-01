using System.Net;
using Oyez.Adapters.Tests.WriteAs;
using Oyez.Core.Channels;

namespace Oyez.Adapters.Tests.Logging;

public class LoggingErrorBodyTests
{
    [Fact]
    public async Task When_logging_an_error_body_should_redact_secrets()
    {
        //Arrange
        var logs = new CollectedLogs();
        using var host = new OyezAdaptersHost(WriteAsTestData.Configure, logs.CaptureIn);
        host.Http.RespondWith(HttpStatusCode.Unauthorized, """{"code":401,"error_msg":"Invalid access token writeas-secret."}""");

        //Act
        await host.PublisherFor(ChannelId.WriteAs).CreateAsync(WriteAsTestData.Article(), CancellationToken.None);

        //Assert
        var rejection = Assert.Single(logs.Entries, entry => entry.Message.Contains("Invalid access token"));
        Assert.Contains("401", rejection.Message);
        Assert.Contains("***", rejection.Message);
        Assert.DoesNotContain(logs.Entries, entry => entry.Message.Contains("writeas-secret"));
    }
}
