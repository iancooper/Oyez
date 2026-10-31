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

using System.Net.Http.Json;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters;

internal sealed class WriteAsPublisher(HttpClient http, WriteAsOptions options) : IAmAChannelPublisher
{
    public ChannelId Channel => ChannelId.WriteAs;

    public bool CreateIsIdempotent => false;

    public async Task<PostResult> CreateAsync(PostRequest request, CancellationToken cancellationToken)
    {
        var post = new { title = request.Rendering.Title, body = request.Rendering.Posts[request.Index].Text };
        using var message = new HttpRequestMessage(HttpMethod.Post, $"{options.ApiBase}/collections/{options.Collection}/posts")
        {
            Content = JsonContent.Create(post)
        };
        message.Headers.TryAddWithoutValidation("Authorization", $"Token {options.AccessToken}");

        await http.SendAsync(message, cancellationToken);

        return new PostResult.Indeterminate("The response is not handled yet");
    }

    public Task<CredentialStatus> VerifyAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
}
