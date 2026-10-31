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

    public static PostRequest Request(int index = 0, PostRef? replyTo = null)
    {
        var posts = new[] { "Brighter 10.1 is out.", "It adds batching.", "Read more at the link." }
            .Select((text, i) => new Post(i, text, text.Length))
            .ToList();
        return new(
            new AnnouncementId("brighter-10-1"),
            new Rendering(ChannelId.Mastodon, null, 500, posts),
            index,
            replyTo,
            "sha256:0123456789ab");
    }
}
