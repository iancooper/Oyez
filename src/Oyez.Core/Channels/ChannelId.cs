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

namespace Oyez.Core.Channels;

/// <summary>
/// Identifies one of the platforms Oyez posts to.
/// </summary>
public readonly record struct ChannelId
{
    /// <summary>X (formerly Twitter).</summary>
    public static readonly ChannelId X = new("x");

    /// <summary>Mastodon.</summary>
    public static readonly ChannelId Mastodon = new("mastodon");

    /// <summary>LinkedIn, posting to a personal profile.</summary>
    public static readonly ChannelId LinkedIn = new("linkedin");

    /// <summary>Write.as, posting to a blog.</summary>
    public static readonly ChannelId WriteAs = new("writeas");

    private ChannelId(string value) => Value = value;

    /// <summary>Gets every channel, in the order Oyez shows and publishes them.</summary>
    /// <value>The <see cref="ChannelId"/>s for X, Mastodon, LinkedIn and Write.as.</value>
    public static IReadOnlyList<ChannelId> All { get; } = [X, Mastodon, LinkedIn, WriteAs];

    /// <summary>Gets the channel's name, as written in announcements and configuration.</summary>
    /// <value>A lower-case <see cref="string"/>, such as <c>x</c> or <c>writeas</c>.</value>
    public string Value { get; }

    /// <summary>Finds the channel with a given name, ignoring case and surrounding whitespace.</summary>
    /// <param name="name">The name to look for, such as <c>WriteAs</c>.</param>
    /// <param name="channel">The matching <see cref="ChannelId"/>, if there is one.</param>
    /// <returns><c>true</c> if <paramref name="name"/> names a channel.</returns>
    public static bool TryParse(string name, out ChannelId channel)
    {
        var normalised = name.Trim().ToLowerInvariant();
        var isChannel = All.Any(c => c.Value == normalised);
        channel = isChannel ? new ChannelId(normalised) : default;
        return isChannel;
    }

    /// <summary>Returns the channel's name.</summary>
    /// <returns>The <see cref="Value"/>.</returns>
    public override string ToString() => Value;
}
