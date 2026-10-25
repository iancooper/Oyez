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
    private (int From, PostResult Result)? _response;
    private CredentialStatus _credentials = new(true, "Credentials are valid.");

    /// <inheritdoc />
    public ChannelId Channel { get; } = channel;

    /// <inheritdoc />
    public bool CreateIsIdempotent => true;

    /// <summary>Gets the requests the publisher has been given.</summary>
    /// <value>The <see cref="PostRequest"/>s, in the order they arrived.</value>
    public IReadOnlyList<PostRequest> Requests => _requests;

    /// <summary>Makes the publisher answer posts with a given result, as a platform does when it rejects them or a request times out.</summary>
    /// <param name="index">The first post index to answer with <paramref name="result"/>. Earlier posts are still made.</param>
    /// <param name="result">The <see cref="PostResult"/> to return instead of posting. A later call replaces an earlier one.</param>
    public void RespondFrom(int index, PostResult result) => _response = (index, result);

    /// <summary>Makes the publisher make every post again.</summary>
    public void RespondNormally() => _response = null;

    /// <summary>Sets what the publisher reports when asked to verify its credentials.</summary>
    /// <param name="status">The <see cref="CredentialStatus"/> to report. Credentials are valid until this is called.</param>
    public void ReportCredentials(CredentialStatus status) => _credentials = status;

    /// <inheritdoc />
    public Task<CredentialStatus> VerifyAsync(CancellationToken cancellationToken) => Task.FromResult(_credentials);

    /// <inheritdoc />
    public Task<PostResult> CreateAsync(PostRequest request, CancellationToken cancellationToken)
    {
        _requests.Add(request);
        if (request.Index >= _response?.From)
        {
            return Task.FromResult(_response.Value.Result);
        }

        var post = new PostRef($"{Channel}-{_requests.Count}", $"https://{Channel}.example/{request.Announcement}/{_requests.Count}");
        return Task.FromResult<PostResult>(new PostResult.Posted(post));
    }
}
