using Oyez.Core.Announcements;

namespace Oyez.Core.Tests;

internal static class PublishingTestData
{
    public static AnnouncementDocument Announcement(string id, string channels, string? publish, params string[] segments)
    {
        var publishLine = publish is null ? string.Empty : $"publish: {publish}\n";
        var text = $"title: Release\nchannels: {channels}\n{publishLine}---\n{string.Join("\n---\n", segments)}";
        return new AnnouncementDocument(new AnnouncementId(id), text, WriteAsMarkdown: null);
    }
}
