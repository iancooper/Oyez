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

namespace Oyez.Core.Counting;

internal static class EmojiRanges
{
    private const string RESOURCE_NAME = "emoji-data.txt";
    private const int COMBINING_ENCLOSING_KEYCAP = 0x20E3;

    private static readonly string[] EMOJI_PROPERTIES = ["Extended_Pictographic", "Emoji_Presentation"];

    private static readonly Lazy<(int Start, int End)[]> RANGES = new(Load);

    public static bool IsEmojiCluster(string cluster) =>
        cluster.EnumerateRunes().Any(rune => IsEmoji(rune.Value));

    private static bool IsEmoji(int codePoint) =>
        codePoint == COMBINING_ENCLOSING_KEYCAP || RANGES.Value.Any(range => codePoint >= range.Start && codePoint <= range.End);

    // The embedded data is Unicode's emoji-data.txt, version 18.0.0 (30 January 2026).
    private static (int Start, int End)[] Load()
    {
        using var stream = typeof(EmojiRanges).Assembly.GetManifestResourceStream(RESOURCE_NAME)
                           ?? throw new InvalidOperationException($"The embedded resource {RESOURCE_NAME} is missing.");
        using var reader = new StreamReader(stream);
        var lines = reader.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return lines.Select(ParseRange).OfType<(int Start, int End)>().ToArray();
    }

    private static (int Start, int End)? ParseRange(string line)
    {
        var fields = line.Split('#')[0].Split(';', StringSplitOptions.TrimEntries);
        return fields.Length == 2 && EMOJI_PROPERTIES.Contains(fields[1]) ? ToRange(fields[0]) : null;
    }

    private static (int Start, int End) ToRange(string codePoints)
    {
        var bounds = codePoints.Split("..");
        var start = int.Parse(bounds[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        var end = int.Parse(bounds[^1], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        return (start, end);
    }
}
