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

using Oyez.Core.Channels;

namespace Oyez.Core.Publishing;

/// <summary>
/// Makes posts on one platform.
/// </summary>
public interface IAmAChannelPublisher
{
    /// <summary>Gets the channel this publisher posts to.</summary>
    /// <value>The <see cref="ChannelId"/> of the platform.</value>
    ChannelId Channel { get; }

    /// <summary>Gets whether the platform uses a post's idempotency key to avoid duplicates.</summary>
    /// <value><c>true</c> when trying the same post again cannot make it twice.</value>
    bool CreateIsIdempotent { get; }

    /// <summary>Makes a post.</summary>
    /// <param name="request">The <see cref="PostRequest"/> to carry out.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>A <see cref="PostResult"/> saying whether the post was made.</returns>
    Task<PostResult> CreateAsync(PostRequest request, CancellationToken cancellationToken);
}
