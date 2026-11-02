using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.Mastodon;

public static class MastodonTestData
{
    public static void Configure(AdapterOptions options)
    {
        options.Mastodon.AccessToken = "mastodon-token";
        options.Mastodon.Instance = "https://hachyderm.io";
    }
}
