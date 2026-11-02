using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.PublicationRecords;

internal static class PublicationRecordTestData
{
    public static PublicationRecord RecordWithEveryState() => new(new Dictionary<ChannelId, ChannelPublication>
    {
        [ChannelId.X] = new(
            ChannelState.Partial,
            new RenderingHash("sha256:aaaa"),
            [new PublishedPost(0, new PostRef("100", "https://x.com/brighter/status/100"), At(10, 0))]),
        [ChannelId.Mastodon] = new(
            ChannelState.Complete,
            new RenderingHash("sha256:bbbb"),
            [
                new PublishedPost(0, new PostRef("200", "https://hachyderm.io/@brighter/200"), At(10, 1)),
                new PublishedPost(1, new PostRef("201", null), At(10, 2))
            ]),
        [ChannelId.LinkedIn] = new(
            ChannelState.Indeterminate,
            new RenderingHash("sha256:cccc"),
            [],
            "LinkedIn timed out while posting"),
        [ChannelId.WriteAs] = new(
            ChannelState.Failed,
            new RenderingHash("sha256:dddd"),
            [new PublishedPost(0, new PostRef("300", "https://write.as/brighter/300"), At(10, 3))],
            "Write.as rejected the post")
    });

    public static void AssertSame(PublicationRecord expected, PublicationRecord actual)
    {
        Assert.Equal(expected.Channels.Keys.OrderBy(c => c.Value), actual.Channels.Keys.OrderBy(c => c.Value));
        foreach (var (channel, publication) in expected.Channels)
        {
            var loaded = actual.Channels[channel];
            Assert.Equal(publication.State, loaded.State);
            Assert.Equal(publication.RenderingHash, loaded.RenderingHash);
            Assert.Equal(publication.Reason, loaded.Reason);
            Assert.Equal(publication.Posts, loaded.Posts);
        }
    }

    private static DateTimeOffset At(int hour, int minute) =>
        new(2026, 11, 3, hour, minute, 0, TimeSpan.FromHours(1));
}
