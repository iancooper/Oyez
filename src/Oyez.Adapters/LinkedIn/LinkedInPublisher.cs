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
using Microsoft.Extensions.Logging;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters;

internal sealed class LinkedInPublisher(HttpClient http, LinkedInOptions options, TimeProvider clock, ILogger<LinkedInPublisher> logger) : IAmAChannelPublisher
{
    private static readonly TimeSpan EXPIRY_WARNING_PERIOD = TimeSpan.FromDays(14);

    public ChannelId Channel => ChannelId.LinkedIn;

    public bool CreateIsIdempotent => false;

    public async Task<PostResult> CreateAsync(PostRequest request, CancellationToken cancellationToken)
    {
        if (TokenHasExpired())
        {
            return new PostResult.Rejected($"LinkedIn token expired on {options.TokenExpiresAt:yyyy-MM-dd}; run oyez auth linkedin");
        }
        WarnIfTokenExpiresSoon();
        using var message = new HttpRequestMessage(HttpMethod.Post, $"{options.ApiBase}/rest/posts");
        message.Content = JsonContent.Create(PostBodyFor(request));
        AddHeaders(message);
        return await PostSender.SendAsync(http, message, "LinkedIn", PostResultFromAsync, cancellationToken);
    }

    public async Task<CredentialStatus> VerifyAsync(CancellationToken cancellationToken)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, $"{options.ApiBase}/v2/userinfo");
        message.Options.Set(RetryHandler.IDEMPOTENT, true);
        AddHeaders(message);
        using var response = await http.SendAsync(message, cancellationToken);
        return new CredentialStatus(response.IsSuccessStatusCode, $"LinkedIn returned {(int)response.StatusCode}", options.TokenExpiresAt);
    }

    private bool TokenHasExpired() =>
        options.TokenExpiresAt is not null && options.TokenExpiresAt <= clock.GetUtcNow();

    private void WarnIfTokenExpiresSoon()
    {
        var expiresAt = options.TokenExpiresAt;
        if (expiresAt is not null && expiresAt <= clock.GetUtcNow() + EXPIRY_WARNING_PERIOD)
        {
            logger.LogWarning("The LinkedIn token expires on {ExpiresAt:yyyy-MM-dd}; run oyez auth linkedin", expiresAt);
        }
    }

    private void AddHeaders(HttpRequestMessage message)
    {
        message.Headers.TryAddWithoutValidation("Authorization", $"Bearer {options.AccessToken}");
        message.Headers.TryAddWithoutValidation("LinkedIn-Version", options.ApiVersion);
        message.Headers.TryAddWithoutValidation("X-Restli-Protocol-Version", "2.0.0");
    }

    private object PostBodyFor(PostRequest request) =>
        new
        {
            author = options.PersonUrn,
            commentary = request.Rendering.Posts[request.Index].Text,
            visibility = "PUBLIC",
            distribution = new { feedDistribution = "MAIN_FEED", targetEntities = Array.Empty<string>(), thirdPartyDistributionChannels = Array.Empty<string>() },
            lifecycleState = "PUBLISHED",
            isReshareDisabledByAuthor = false
        };

    private static async Task<PostResult> PostResultFromAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            return new PostResult.Rejected(await RejectionReasonAsync(response, cancellationToken));
        }
        var urn = response.Headers.GetValues("x-restli-id").Single();
        return new PostResult.Posted(new PostRef(urn, $"https://www.linkedin.com/feed/update/{urn}"));
    }

    private static async Task<string> RejectionReasonAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var status = $"LinkedIn returned {(int)response.StatusCode}";
        var message = MessageFrom(await response.Content.ReadAsStringAsync(cancellationToken));
        return message is null ? status : $"{status}: {message}";
    }

    private static string? MessageFrom(string body)
    {
        try
        {
            using var json = JsonDocument.Parse(body);
            return json.RootElement.TryGetProperty("message", out var message) ? message.GetString() : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
