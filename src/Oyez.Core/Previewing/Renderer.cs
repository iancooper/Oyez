using Oyez.Core.Announcements;
using Oyez.Core.Channels;

namespace Oyez.Core.Previewing;

internal static class Renderer
{
    public static IReadOnlyList<Rendering> RenderTargetedChannels(Announcement announcement) =>
        ChannelId.All
            .Where(announcement.Channels.Contains)
            .Select(channel => Render(announcement, channel))
            .ToList();

    private static Rendering Render(Announcement announcement, ChannelId channel) =>
        channel == ChannelId.WriteAs
            ? RenderDocument(announcement, channel)
            : RenderPosts(announcement, channel);

    private static Rendering RenderDocument(Announcement announcement, ChannelId channel) =>
        new(channel, announcement.Title, [new Post(0, announcement.WriteAsMarkdown ?? string.Empty)]);

    private static Rendering RenderPosts(Announcement announcement, ChannelId channel) =>
        new(channel, null, announcement.Segments.Select((segment, index) => new Post(index, segment.Text)).ToList());
}
