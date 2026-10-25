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
/// Pretends to post to a platform, and remembers the requests it was given. Use it in tests, or for a dry run.
/// </summary>
/// <param name="channel">The channel to pretend to post to.</param>
public sealed class InMemoryChannelPublisher(ChannelId channel) : IAmAChannelPublisher
{
    private readonly List<PostRequest> _requests = [];
    private int? _rejectFrom;
    private string _rejection = string.Empty;
    private int? _indeterminateFrom;
    private string _uncertainty = string.Empty;

    /// <inheritdoc />
    public ChannelId Channel { get; } = channel;

    /// <inheritdoc />
    public bool CreateIsIdempotent => true;

    /// <summary>Gets the requests the publisher has been given.</summary>
    /// <value>The <see cref="PostRequest"/>s, in the order they arrived.</value>
    public IReadOnlyList<PostRequest> Requests => _requests;

    /// <summary>Makes the publisher refuse posts, as a platform does when it rejects them or a secret is missing.</summary>
    /// <param name="index">The first post index to refuse. Earlier posts are still made.</param>
    /// <param name="reason">The reason to give for the refusal.</param>
    public void RejectFrom(int index, string reason)
    {
        _rejectFrom = index;
        _rejection = reason;
    }

    /// <summary>Makes the publisher accept every post again.</summary>
    public void StopRejecting() => _rejectFrom = null;

    /// <summary>Makes the publisher fail to say whether a post was made, as a platform does when a request times out.</summary>
    /// <param name="index">The first post index to leave in doubt. Earlier posts are still made.</param>
    /// <param name="reason">The reason to give for the doubt.</param>
    public void IndeterminateFrom(int index, string reason)
    {
        _indeterminateFrom = index;
        _uncertainty = reason;
    }

    /// <inheritdoc />
    public Task<PostResult> CreateAsync(PostRequest request, CancellationToken cancellationToken)
    {
        _requests.Add(request);
        if (request.Index >= _rejectFrom)
        {
            return Task.FromResult<PostResult>(new PostResult.Rejected(_rejection));
        }

        if (request.Index >= _indeterminateFrom)
        {
            return Task.FromResult<PostResult>(new PostResult.Indeterminate(_uncertainty));
        }

        var post = new PostRef($"{Channel}-{_requests.Count}", $"https://{Channel}.example/{request.Announcement}/{_requests.Count}");
        return Task.FromResult<PostResult>(new PostResult.Posted(post));
    }
}
