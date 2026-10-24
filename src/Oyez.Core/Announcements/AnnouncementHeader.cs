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

    private static readonly string[] KNOWN_KEYS = [TITLE_KEY, PUBLISH_KEY, CHANNELS_KEY];
    private static readonly string[] ISO_8601_FORMATS =
        ["yyyy-MM-dd'T'HH:mmK", "yyyy-MM-dd'T'HH:mm:ssK", "yyyy-MM-dd'T'HH:mm:ss.FFFFFFFK"];

    private readonly AnnouncementId _id;
    private readonly List<ValidationError> _errors = [];
    private readonly Dictionary<string, string> _entries;

    private AnnouncementHeader(AnnouncementId id, IEnumerable<string> lines)
    {
        _id = id;
        _entries = ReadEntries(lines);
        Title = ReadTitle();
        PublishAt = ReadPublishTime();
        Channels = ReadChannels();
    }

    public string Title { get; }

    public DateTimeOffset? PublishAt { get; }

    public IReadOnlySet<ChannelId> Channels { get; }

    public IReadOnlyList<ValidationError> Errors => _errors;

    public static AnnouncementHeader Parse(AnnouncementId id, IEnumerable<string> lines) => new(id, lines);

    private Dictionary<string, string> ReadEntries(IEnumerable<string> lines)
    {
        var entries = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var line in lines.Select(line => line.Trim()).Where(IsEntry))
        {
            AddEntry(entries, line);
        }

        return entries;
    }

    private static bool IsEntry(string line) => line.Length > 0 && !line.StartsWith('#');

    private void AddEntry(Dictionary<string, string> entries, string line)
    {
        var colon = line.IndexOf(':');
        if (colon < 0)
            Report($"The header line '{line}' has no colon. Write it as 'key: value', or, if it belongs in a segment, put a '---' line before it.");
        else
            AddKnownEntry(entries, line[..colon].Trim(), line[(colon + 1)..].Trim());
    }

    private void AddKnownEntry(Dictionary<string, string> entries, string key, string value)
    {
        if (KNOWN_KEYS.Contains(key, StringComparer.OrdinalIgnoreCase))
            entries[key] = value;
        else
            Report($"The header key '{key}' isn't known. Use {string.Join(", ", KNOWN_KEYS)}.");
    }

    private string ReadTitle()
    {
        var title = _entries.GetValueOrDefault(TITLE_KEY, string.Empty);
        if (title.Length == 0)
            Report("The announcement has no title. Add a header line such as 'title: Brighter 10.3.0 released'.");

        return title;
    }

    private DateTimeOffset? ReadPublishTime() =>
        _entries.TryGetValue(PUBLISH_KEY, out var publish) ? ParsePublishTime(publish) : null;

    // A time without "Z" or an offset would be read in the machine's time zone, so it is rejected.
    private DateTimeOffset? ParsePublishTime(string publish)
    {
        if (!DateTime.TryParseExact(publish, ISO_8601_FORMATS, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var time))
            return Reject($"The publish time '{publish}' isn't an ISO 8601 time, such as 2026-11-03T09:00Z.");

        if (time.Kind == DateTimeKind.Unspecified)
            return Reject($"The publish time '{publish}' has no time zone. Add Z for UTC, or an offset such as +01:00.");

        return DateTimeOffset.ParseExact(publish, ISO_8601_FORMATS, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
    }

    private HashSet<ChannelId> ReadChannels() =>
        _entries.TryGetValue(CHANNELS_KEY, out var names) ? ParseChannels(names) : ChannelId.All.ToHashSet();

    private HashSet<ChannelId> ParseChannels(string names)
    {
        var channels = new HashSet<ChannelId>();
        foreach (var name in names.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            AddChannel(channels, name);
        }

        return channels;
    }

    private void AddChannel(HashSet<ChannelId> channels, string name)
    {
        if (ChannelId.TryParse(name, out var channel))
            channels.Add(channel);
        else
            Report($"'{name}' isn't a channel. Use {string.Join(", ", ChannelId.All)}.");
    }

    private DateTimeOffset? Reject(string message)
    {
        Report(message);
        return null;
    }

    private void Report(string message) => _errors.Add(new ValidationError(_id, message));
}
