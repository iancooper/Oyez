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

namespace Oyez.Core.Counting;

internal sealed partial class UrlMatcher
{
    private const int LINK_LENGTH = 23;

    private const string PLACEHOLDER = "x";

    private readonly bool _includeBareDomains;

    private UrlMatcher(bool includeBareDomains) => _includeBareDomains = includeBareDomains;

    public static UrlMatcher ForX { get; } = new(includeBareDomains: true);

    public static UrlMatcher ForMastodon { get; } = new(includeBareDomains: false);

    public string ReplaceWithPlaceholders(string text) =>
        LinkPattern().Replace(text, ReplacementFor);

    public bool ContainsLink(string text) =>
        LinkPattern().Matches(text).Any(IsLink);

    private string ReplacementFor(Match match) =>
        IsLink(match) ? string.Concat(Enumerable.Repeat(PLACEHOLDER, LINK_LENGTH)) : match.Value;

    private bool IsLink(Match match) =>
        match.Groups["scheme"].Success || (_includeBareDomains && TopLevelDomains.Contains(match.Groups["tld"].Value));

    [GeneratedRegex(
        """
        (?<scheme>https?://\S*[^\s.,;:!?'"()\[\]{}<>])
        |
        (?<![\p{L}\p{N}_@./:-])
        (?:[\p{L}\p{N}](?:[\p{L}\p{N}-]*[\p{L}\p{N}])?\.)+(?<tld>[\p{L}\p{N}](?:[\p{L}\p{N}-]*[\p{L}\p{N}])?)
        (?![\p{L}\p{N}_@-])
        (?:/(?:\S*[^\s.,;:!?'"()\[\]{}<>])?)?
        """,
        RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex LinkPattern();
}
