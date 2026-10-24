#region Licence

/* The MIT License (MIT)
Copyright © 2026 Ian Cooper

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE. */

#endregion

using System.Globalization;
using Oyez.Core.Channels;

namespace Oyez.Core.Announcements;

// The "key: value" lines before the first "---". Keys are case-insensitive; blank lines and "#" comments are ignored.
internal sealed class AnnouncementHeader
{
    private const string TITLE_KEY = "title";
    private const string PUBLISH_KEY = "publish";
    private const string CHANNELS_KEY = "channels";

    private readonly Dictionary<string, string> _entries;

    private AnnouncementHeader(IEnumerable<string> lines)
    {
        _entries = ReadEntries(lines);
        Title = _entries.GetValueOrDefault(TITLE_KEY, string.Empty);
        PublishAt = ReadPublishTime();
        Channels = ReadChannels();
    }

    public string Title { get; }

    public DateTimeOffset? PublishAt { get; }

    public IReadOnlySet<ChannelId> Channels { get; }

    public static AnnouncementHeader Parse(IEnumerable<string> lines) => new(lines);

    private static Dictionary<string, string> ReadEntries(IEnumerable<string> lines) =>
        lines
            .Select(line => line.Trim())
            .Where(IsEntry)
            .Select(SplitEntry)
            .GroupBy(entry => entry.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Last().Value, StringComparer.OrdinalIgnoreCase);

    private static bool IsEntry(string line) => line.Length > 0 && !line.StartsWith('#');

    private static KeyValuePair<string, string> SplitEntry(string line)
    {
        var colon = line.IndexOf(':');
        return new(line[..colon].Trim(), line[(colon + 1)..].Trim());
    }

    private DateTimeOffset? ReadPublishTime() =>
        _entries.TryGetValue(PUBLISH_KEY, out var publish)
            ? DateTimeOffset.Parse(publish, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal)
            : null;

    private HashSet<ChannelId> ReadChannels() =>
        _entries.TryGetValue(CHANNELS_KEY, out var names)
            ? names.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(ToChannel).ToHashSet()
            : ChannelId.All.ToHashSet();

    private static ChannelId ToChannel(string name) => ChannelId.TryParse(name, out var channel) ? channel : default;
}
