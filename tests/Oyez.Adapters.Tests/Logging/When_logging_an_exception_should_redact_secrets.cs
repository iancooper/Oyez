using Oyez.Adapters.Tests.WriteAs;
using Oyez.Core.Channels;

namespace Oyez.Adapters.Tests.Logging;

public class LoggingExceptionTests
{
    [Fact]
    public async Task When_logging_an_exception_should_redact_secrets()
    {
        //Arrange
        var logs = new CollectedLogs();
        using var host = new OyezAdaptersHost(WriteAsTestData.Configure, logs.CaptureIn);
        host.Http.Throw(new HttpRequestException("Connection to https://write.as/api?token=writeas-secret failed"));

        //Act
        await Assert.ThrowsAsync<HttpRequestException>(() =>
            host.PublisherFor(ChannelId.WriteAs).CreateAsync(WriteAsTestData.Article(), CancellationToken.None));

        //Assert
        var failure = Assert.Single(logs.Entries, entry => entry.Message.Contains("Connection to"));
        Assert.Contains("***", failure.Message);
        Assert.DoesNotContain(logs.Entries, entry => entry.Message.Contains("writeas-secret"));
    }
}
