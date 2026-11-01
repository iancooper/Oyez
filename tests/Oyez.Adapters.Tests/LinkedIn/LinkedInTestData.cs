using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.LinkedIn;

public static class LinkedInTestData
{
    public static readonly DateTimeOffset NOW = new(2026, 10, 31, 9, 0, 0, TimeSpan.Zero);

    public static void Configure(AdapterOptions options)
    {
        options.LinkedIn.AccessToken = "linkedin-token";
        options.LinkedIn.PersonUrn = "urn:li:person:8675309";
        options.LinkedIn.ApiBase = "https://api.linkedin.com";
        options.LinkedIn.ApiVersion = "202609";
        options.LinkedIn.TokenExpiresAt = NOW.AddDays(60);
    }

    public static Action<AdapterOptions> ConfigureExpiringAt(DateTimeOffset expiresAt) =>
        options =>
        {
            Configure(options);
            options.LinkedIn.TokenExpiresAt = expiresAt;
        };

    public static void FixTime(IServiceCollection services)
    {
        var clock = new FakeTimeProvider();
        clock.SetUtcNow(NOW);
        services.AddSingleton<TimeProvider>(clock);
    }

    public static HttpResponseMessage CreatedWithId(string urn)
    {
        var response = new HttpResponseMessage(HttpStatusCode.Created);
        response.Headers.Add("x-restli-id", urn);
        return response;
    }
}
