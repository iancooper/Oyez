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

using System.Text.RegularExpressions;
using Oyez.Core.Configuration;
using Oyez.Core.Counting;

namespace Oyez.Core.Channels;

internal sealed partial class LinkedInProfile(OyezOptions options) : IAmAChannelProfile
{
    private const string RESERVED_CHARACTERS = @"[\\|{}@\[\]()<>#*_~]";
    private const string HASHTAG = @"(?<![\p{L}\p{N}_])#(?<tag>[\p{L}\p{N}_]+)";

    public ChannelId Id => ChannelId.LinkedIn;
    public RenderingPolicy Policy => RenderingPolicy.SinglePost;
    public bool IsEnabled => options.LinkedIn.Enabled;
    public int? Limit => options.LinkedIn.Limit;
    public IAmATextCounter Counter { get; } = new CodeUnitTextCounter();

    public string Prepare(string text) =>
        (options.LinkedIn.ConvertHashtags ? HashtagsAndReservedCharacters() : ReservedCharacters())
            .Replace(text, ConvertOrEscape);

    private static string ConvertOrEscape(Match match) =>
        match.Groups["tag"].Success
            ? $"{{hashtag|\\#|{match.Groups["tag"].Value}}}"
            : $"\\{match.Value}";

    [GeneratedRegex(HASHTAG + "|" + RESERVED_CHARACTERS)]
    private static partial Regex HashtagsAndReservedCharacters();

    [GeneratedRegex(RESERVED_CHARACTERS)]
    private static partial Regex ReservedCharacters();
}
