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
        channel = All.FirstOrDefault(c => c.Value == normalised);
        return channel.Value is not null;
    }

    /// <summary>Returns the channel's name.</summary>
    /// <returns>The <see cref="Value"/>.</returns>
    public override string ToString() => Value;
}
