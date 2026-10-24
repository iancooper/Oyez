namespace Oyez.Core.Tests;

internal static class AnnouncementText
{
    public static string OfLength(int length, char fill) => new(fill, length);

    public static string With(string channels, params string[] segments) =>
        $"title: Release\nchannels: {channels}\n---\n{string.Join("\n---\n", segments)}";
}
