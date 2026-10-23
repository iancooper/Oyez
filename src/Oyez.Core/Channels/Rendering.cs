namespace Oyez.Core.Channels;

/// <summary>
/// The posts an announcement becomes on one channel.
/// </summary>
/// <param name="Channel">The <see cref="ChannelId"/> the posts are for.</param>
/// <param name="Title">The post's title, for channels that have titles (Write.as); otherwise <c>null</c>.</param>
/// <param name="Posts">The <see cref="Post"/>s, in publishing order. A thread has more than one.</param>
public sealed record Rendering(ChannelId Channel, string? Title, IReadOnlyList<Post> Posts);
