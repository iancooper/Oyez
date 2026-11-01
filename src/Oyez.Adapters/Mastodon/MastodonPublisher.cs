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

internal sealed class MastodonPublisher(HttpClient http, MastodonOptions options) : IAmAChannelPublisher
{
    private readonly PlatformClient _platform = new(http, "Mastodon");

    public ChannelId Channel => ChannelId.Mastodon;

    public bool CreateIsIdempotent => true;

    public async Task<PostResult> CreateAsync(PostRequest request, CancellationToken cancellationToken)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, $"{options.Instance}/api/v1/statuses");
        message.Content = JsonContent.Create(PostBodyFor(request));
        message.Options.Set(RetryHandler.IDEMPOTENT, true);
        message.Headers.TryAddWithoutValidation("Idempotency-Key", request.IdempotencyKey);
        Authorise(message);
        return await _platform.SendPostAsync(message, PostResultFromAsync, cancellationToken);
    }

    public Task<CredentialStatus> VerifyAsync(CancellationToken cancellationToken) =>
        _platform.VerifyAsync($"{options.Instance}/api/v1/accounts/verify_credentials", Authorise, null, cancellationToken);

    private void Authorise(HttpRequestMessage message) =>
        message.Headers.TryAddWithoutValidation("Authorization", $"Bearer {options.AccessToken}");

    private static Dictionary<string, object> PostBodyFor(PostRequest request)
    {
        var body = new Dictionary<string, object>
        {
            ["status"] = request.PostText(),
            ["visibility"] = "public"
        };
        if (request.ReplyTo is not null)
        {
            body["in_reply_to_id"] = request.ReplyTo.Id;
        }
        return body;
    }

    private async Task<PostResult> PostResultFromAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        using var body = await PlatformClient.ReadJsonAsync(response, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return _platform.RejectedBecause(response, body.RootElement.GetProperty("error").GetString());
        }
        return new PostResult.Posted(new PostRef(body.RootElement.GetProperty("id").GetString()!, body.RootElement.GetProperty("url").GetString()!));
    }
}
