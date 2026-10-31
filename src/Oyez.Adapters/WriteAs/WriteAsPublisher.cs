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
using System.Text.Json;
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
        using var message = Authorised(HttpMethod.Post, $"collections/{options.Collection}/posts");
        message.Content = JsonContent.Create(post);

        return await PostSender.SendAsync(http, message, "Write.as", PostResultFromAsync, cancellationToken);
    }

    public async Task<CredentialStatus> VerifyAsync(CancellationToken cancellationToken)
    {
        using var message = Authorised(HttpMethod.Get, "me");
        message.Options.Set(RetryHandler.IDEMPOTENT, true);

        using var response = await http.SendAsync(message, cancellationToken);

        return new CredentialStatus(response.IsSuccessStatusCode, $"Write.as returned {(int)response.StatusCode}");
    }

    private HttpRequestMessage Authorised(HttpMethod method, string path)
    {
        var message = new HttpRequestMessage(method, $"{options.ApiBase}/{path}");
        message.Headers.TryAddWithoutValidation("Authorization", $"Token {options.AccessToken}");
        return message;
    }

    private async Task<PostResult> PostResultFromAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        using var body = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return new PostResult.Rejected($"Write.as returned {(int)response.StatusCode}: {body.RootElement.GetProperty("error_msg").GetString()}");
        }

        var data = body.RootElement.GetProperty("data");
        var slug = data.GetProperty("slug").GetString();
        return new PostResult.Posted(new PostRef(data.GetProperty("id").GetString()!, $"{options.BlogUrl}/{slug}"));
    }
}
