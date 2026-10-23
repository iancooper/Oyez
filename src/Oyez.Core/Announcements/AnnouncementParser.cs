using System.Globalization;
using System.Text;
using Oyez.Core.Channels;

namespace Oyez.Core.Announcements;

// Reads announcement.txt: a "key: value" header, then plain-text segments separated by lines that are exactly "---".
internal static class AnnouncementParser
{
    private const string SEPARATOR = "---";

    public static Result<Announcement> Parse(AnnouncementDocument document)
    {
        var blocks = SplitIntoBlocks(Normalise(document.Text));
        var header = ParseHeader(blocks[0]);

        return Result<Announcement>.Success(new Announcement(
            document.Id,
            header.GetValueOrDefault("title", string.Empty),
            ParsePublishTime(header),
            ParseChannels(header),
            ToSegments(blocks.Skip(1)),
            document.WriteAsMarkdown is null ? null : Normalise(document.WriteAsMarkdown)));
    }

    private static string Normalise(string text) =>
        text.Replace("\r\n", "\n").Replace('\r', '\n').Normalize(NormalizationForm.FormC);

    private static List<List<string>> SplitIntoBlocks(string text)
    {
        var blocks = new List<List<string>> { new() };
        foreach (var line in text.Split('\n'))
        {
            AddToBlocks(blocks, line);
        }

        return blocks;
    }

    private static void AddToBlocks(List<List<string>> blocks, string line)
    {
        if (IsSeparator(line))
            blocks.Add([]);
        else
            blocks[^1].Add(line);
    }

    private static bool IsSeparator(string line) => line.TrimEnd() == SEPARATOR;

    private static Dictionary<string, string> ParseHeader(IEnumerable<string> lines) =>
        lines
            .Select(line => line.Trim())
            .Where(IsHeaderEntry)
            .Select(SplitHeaderEntry)
            .GroupBy(entry => entry.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Last().Value, StringComparer.OrdinalIgnoreCase);

    private static bool IsHeaderEntry(string line) => line.Length > 0 && !line.StartsWith('#');

    private static KeyValuePair<string, string> SplitHeaderEntry(string line)
    {
        var colon = line.IndexOf(':');
        return new(line[..colon].Trim(), line[(colon + 1)..].Trim());
    }

    private static DateTimeOffset? ParsePublishTime(Dictionary<string, string> header) =>
        header.TryGetValue("publish", out var publish)
            ? DateTimeOffset.Parse(publish, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal)
            : null;

    private static HashSet<ChannelId> ParseChannels(Dictionary<string, string> header) =>
        header.TryGetValue("channels", out var names)
            ? names.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(ToChannel).ToHashSet()
            : ChannelId.All.ToHashSet();

    private static ChannelId ToChannel(string name) => ChannelId.TryParse(name, out var channel) ? channel : default;

    private static List<Segment> ToSegments(IEnumerable<List<string>> blocks) =>
        blocks.Select((lines, index) => new Segment(index + 1, string.Join('\n', lines).Trim())).ToList();
}
