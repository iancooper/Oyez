using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Oyez.Adapters.Tests.Retries;
using Oyez.Core.Channels;

namespace Oyez.Adapters.Tests.X;

public static class RateLimitedXPostTestData
{
    public static readonly DateTimeOffset START = DateTimeOffset.FromUnixTimeSeconds(1_800_000_000);

    public static async Task<IReadOnlyList<SentRequest>> PostRetriedAfterRateLimitAsync(SchedulingClock clock)
    {
        clock.SetUtcNow(START);
        using var host = new OyezAdaptersHost(
            XTestData.Configure,
            services =>
            {
                services.AddSingleton<TimeProvider>(clock);
                services.AddSingleton<IAmANonceSource>(new SequentialNonceSource());
            });
        host.Http.RespondWith(requestNumber => requestNumber == 1
            ? RetryTestData.RateLimited(retryAfterSeconds: 30)
            : Responses.Respond(HttpStatusCode.OK, """{"data":{"id":"1","text":"Brighter 10.1 is out."}}"""));

        var pending = host.PublisherFor(ChannelId.X).CreateAsync(PostRequests.Thread(ChannelId.X, 280), CancellationToken.None);
        await host.Http.RequestArrivedAsync(1);
        await clock.TimerCreatedAsync(1);
        clock.Advance(TimeSpan.FromSeconds(30));
        await pending;
        return host.Http.Requests.ToList();
    }

    public static string OAuthValue(SentRequest request, string name) =>
        Regex.Match(request.Headers["Authorization"], $"{name}=\"([^\"]*)\"").Groups[1].Value;
}
