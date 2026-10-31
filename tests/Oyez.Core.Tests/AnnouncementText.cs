namespace Oyez.Core.Tests;

internal static class AnnouncementText
{
    public static string OfLength(int length, char fill) => new(fill, length);

    public static string With(string channels, params string[] segments) => Build(channels, null, segments);

    public static string With(string channels, string[] segments, string? publish) => Build(channels, publish, segments);

    private static string Build(string channels, string? publish, string[] segments)
    {
        var publishLine = publish is null ? string.Empty : $"publish: {publish}\n";
        return $"title: Release\nchannels: {channels}\n{publishLine}---\n{string.Join("\n---\n", segments)}";
    }
}
