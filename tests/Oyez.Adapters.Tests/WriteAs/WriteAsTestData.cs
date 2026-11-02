using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.WriteAs;

public static class WriteAsTestData
{
    public static void Configure(AdapterOptions options)
    {
        options.WriteAs.AccessToken = "writeas-secret";
        options.WriteAs.Collection = "ian-cooper";
        options.WriteAs.ApiBase = "https://write.as/api";
        options.WriteAs.BlogUrl = "https://ian-cooper.writeas.com";
    }

    public static PostRequest Article(string title = "Brighter 10.1", string markdown = "Brighter 10.1 is out.\n\nSee what changed.") =>
        PostRequests.Build(ChannelId.WriteAs, null, title, [markdown]);
}
