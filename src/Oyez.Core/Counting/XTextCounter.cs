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
using System.Text;

namespace Oyez.Core.Counting;

internal sealed class XTextCounter : IAmATextCounter
{
    private const int EMOJI_WEIGHT = 2;
    private const int HEAVY_WEIGHT = 2;
    private const int LIGHT_WEIGHT = 1;

    private static readonly (int Start, int End)[] _lightRanges =
    [
        (0x0000, 0x10FF), (0x2000, 0x200D), (0x2010, 0x201F), (0x2032, 0x2037)
    ];

    private readonly UrlMatcher _urls = UrlMatcher.ForX;

    public int Count(string text) =>
        Clusters(_urls.ReplaceWithPlaceholders(text.Normalize(NormalizationForm.FormC))).Sum(WeightOf);

    private static IEnumerable<string> Clusters(string text)
    {
        var elements = StringInfo.GetTextElementEnumerator(text);
        while (elements.MoveNext())
        {
            yield return (string)elements.Current;
        }
    }

    private static int WeightOf(string cluster) =>
        EmojiRanges.IsEmojiCluster(cluster) ? EMOJI_WEIGHT : cluster.EnumerateRunes().Sum(rune => WeightOf(rune.Value));

    private static int WeightOf(int codePoint) =>
        _lightRanges.Any(range => codePoint >= range.Start && codePoint <= range.End) ? LIGHT_WEIGHT : HEAVY_WEIGHT;
}
