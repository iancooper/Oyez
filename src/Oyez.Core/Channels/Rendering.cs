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

using System.Security.Cryptography;
using System.Text;

namespace Oyez.Core.Channels;

/// <summary>
/// The posts an announcement becomes on one channel.
/// </summary>
/// <param name="Channel">The <see cref="ChannelId"/> the posts are for.</param>
/// <param name="Title">The post's title, for channels that have titles (Write.as); otherwise <c>null</c>.</param>
/// <param name="Limit">The most characters a post may have on the channel, or <c>null</c> if it has no limit.</param>
/// <param name="Posts">The <see cref="Post"/>s, in publishing order. A thread has more than one.</param>
public sealed record Rendering(ChannelId Channel, string? Title, int? Limit, IReadOnlyList<Post> Posts)
{
    /// <summary>
    /// Gets a fingerprint of the post texts, in the form <c>sha256:</c> followed by lower-case hexadecimal.
    /// </summary>
    /// <value>The same hash for the same posts, and a different hash if any post's text, or the order of posts, changes.</value>
    public string Hash => $"sha256:{Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(string.Join('\0', Posts.Select(post => post.Text)))))}";
}
