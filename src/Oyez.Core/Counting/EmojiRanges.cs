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

namespace Oyez.Core.Counting;

internal static class EmojiRanges
{
    private const string RESOURCE_NAME = "emoji-data.txt";
    private const int COMBINING_ENCLOSING_KEYCAP = 0x20E3;

    private static readonly string[] EMOJI_PROPERTIES = ["Extended_Pictographic", "Emoji_Presentation"];

    private static readonly Lazy<CodePointRange[]> RANGES = new(Load);

    public static bool IsEmojiCluster(string cluster) =>
        cluster.EnumerateRunes().Any(rune => IsEmoji(rune.Value));

    private static bool IsEmoji(int codePoint) =>
        codePoint == COMBINING_ENCLOSING_KEYCAP || RANGES.Value.Any(range => range.Contains(codePoint));

    // The embedded data is Unicode's emoji-data.txt, version 18.0.0 (30 January 2026).
    private static CodePointRange[] Load() =>
        EmbeddedResource.ReadLines(RESOURCE_NAME).Select(ParseRange).OfType<CodePointRange>().ToArray();

    private static CodePointRange? ParseRange(string line)
    {
        var fields = line.Split('#')[0].Split(';', StringSplitOptions.TrimEntries);
        return fields.Length == 2 && EMOJI_PROPERTIES.Contains(fields[1]) ? CodePointRange.Parse(fields[0]) : null;
    }
}
