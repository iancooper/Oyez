using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
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

    public static Task<PostResult> CreateAsync(OyezAdaptersHost host, ChannelUnderTest channel) =>
        host.PublisherFor(channel.Channel).CreateAsync(channel.Request(), CancellationToken.None);

    public static Task<CredentialStatus> VerifyAsync(OyezAdaptersHost host, ChannelUnderTest channel) =>
        host.PublisherFor(channel.Channel).VerifyAsync(CancellationToken.None);

    public static HttpResponseMessage RateLimited(int retryAfterSeconds)
    {
        var response = Responses.Respond(HttpStatusCode.TooManyRequests, """{"detail":"Too Many Requests","error_msg":"Too Many Requests"}""");
        response.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(TimeSpan.FromSeconds(retryAfterSeconds));
        return response;
    }

    public static HttpRequestException ConnectionRefused() =>
        new("Connection refused", new SocketException((int)SocketError.ConnectionRefused));
}
