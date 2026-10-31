using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.Retries;

public static class RetryTestData
{
    public static OyezAdaptersHost HostWith(FakeTimeProvider clock) =>
        new(
            options =>
            {
                X.XTestData.Configure(options);
                WriteAs.WriteAsTestData.Configure(options);
            },
            services => services.AddSingleton<TimeProvider>(clock));

    public static Task<PostResult> CreateAsync(OyezAdaptersHost host, string channel) =>
        host.PublisherFor(ChannelIdNamed(channel)).CreateAsync(RequestFor(channel), CancellationToken.None);

    public static Task<CredentialStatus> VerifyAsync(OyezAdaptersHost host, string channel) =>
        host.PublisherFor(ChannelIdNamed(channel)).VerifyAsync(CancellationToken.None);

    public static HttpResponseMessage Respond(HttpStatusCode status, string body = "{}") =>
        new(status) { Content = new StringContent(body, Encoding.UTF8, "application/json") };

    public static HttpResponseMessage RateLimited(int retryAfterSeconds)
    {
        var response = Respond(HttpStatusCode.TooManyRequests, """{"detail":"Too Many Requests","error_msg":"Too Many Requests"}""");
        response.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(TimeSpan.FromSeconds(retryAfterSeconds));
        return response;
    }

    public static async Task WaitForRequestsAsync(OyezAdaptersHost host, int count)
    {
        var giveUpAt = DateTime.UtcNow.AddSeconds(2);
        while (host.Http.Requests.Count < count && DateTime.UtcNow < giveUpAt)
        {
            await Task.Delay(5);
        }
    }

    public static string AcceptedBodyFor(string channel) =>
        channel == "x"
            ? """{"data":{"id":"1","text":"Brighter 10.1 is out."}}"""
            : """{"code":201,"data":{"id":"abc","slug":"brighter-10-1"}}""";

    public static HttpRequestException ConnectionRefused() =>
        new("Connection refused", new SocketException((int)SocketError.ConnectionRefused));

    public static async Task<T> CompleteWithClockRunningAsync<T>(Task<T> pending, FakeTimeProvider clock)
    {
        while (!pending.IsCompleted)
        {
            clock.Advance(TimeSpan.FromSeconds(1));
            await Task.Delay(5);
        }
        return await pending;
    }

    private static ChannelId ChannelIdNamed(string channel) =>
        ChannelId.TryParse(channel, out var id) ? id : throw new ArgumentException(channel);

    private static PostRequest RequestFor(string channel) =>
        channel == "x" ? X.XTestData.Request() : WriteAs.WriteAsTestData.Request();
}
