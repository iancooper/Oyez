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

internal readonly record struct CodePointRange(int Start, int End)
{
    public bool Contains(int codePoint) => codePoint >= Start && codePoint <= End;

    // Parses Unicode's data-file notation: a single code point such as "00A9", or a range such as "1F600..1F64F".
    public static CodePointRange Parse(string codePoints)
    {
        var bounds = codePoints.Split("..");
        return new CodePointRange(ParseHex(bounds[0]), ParseHex(bounds[^1]));
    }

    private static int ParseHex(string codePoint) =>
        int.Parse(codePoint, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
}
