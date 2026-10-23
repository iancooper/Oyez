namespace Oyez.Core.Channels;

/// <summary>
/// One post, as it will be published on a channel.
/// </summary>
/// <param name="Index">The post's position in its <see cref="Rendering"/>, counting from 0.</param>
/// <param name="Text">The text to publish.</param>
public sealed record Post(int Index, string Text);
