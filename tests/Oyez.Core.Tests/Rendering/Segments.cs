namespace Oyez.Core.Tests.Rendering;

internal static class Segments
{
    public static string OfLength(int length, char fill) => new(fill, length);

    public static string Announcement(string channels, params string[] segments) =>
        $"title: Release\nchannels: {channels}\n---\n{string.Join("\n---\n", segments)}";
}
