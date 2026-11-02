using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.X;

public static class XTestData
{
    public static void Configure(AdapterOptions options)
    {
        options.X.ConsumerKey = "consumer-key";
        options.X.ConsumerSecret = "consumer-secret";
        options.X.AccessToken = "access-token";
        options.X.AccessTokenSecret = "access-token-secret";
        options.X.ApiBase = "https://api.x.com";
        options.X.Handle = "ICooper";
    }

    public static void ConfigureWithPublishedExampleCredentials(AdapterOptions options)
    {
        Configure(options);
        options.X.ConsumerKey = "xvz1evFS4wEEPTGEFPHBog";
        options.X.ConsumerSecret = "kAcSOqF21Fu85e7zjz7ZN2U4ZRhfV3WpwPAoE3Z7kBw";
        options.X.AccessToken = "370773112-GmHxMAgYyLbNEtIKZeRNFsMKPR9EyMZeS9weJAEb";
        options.X.AccessTokenSecret = "LswwdoUaIvS8ltyTt5jkRh4J50vUPVVHtR2YPi5kE";
    }

    public static void FixPublishedExampleTimeAndNonce(IServiceCollection services)
    {
        var clock = new FakeTimeProvider();
        clock.SetUtcNow(DateTimeOffset.FromUnixTimeSeconds(1318622958));
        services.AddSingleton<TimeProvider>(clock);
        services.AddSingleton<IAmANonceSource>(new FixedNonceSource("kYjzVBB8Y0ZFabxSWbWovY3uYSQ2pTgmZeNu2VS4cg"));
    }
}
