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

using System.Net;
using System.Text.Json;
using Oyez.Core.Publishing;

namespace Oyez.Adapters;

internal sealed class PlatformClient(HttpClient http, string platform)
{
    public async Task<PostResult> SendPostAsync(
        HttpRequestMessage message,
        Func<HttpResponseMessage, CancellationToken, Task<PostResult>> readAccepted,
        CancellationToken cancellationToken)
    {
        try
        {
            using var response = await http.SendAsync(message, cancellationToken);
            return await ResultFromAsync(response, readAccepted, cancellationToken);
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return new PostResult.Indeterminate($"{platform} did not answer in time, so the post may have been made");
        }
    }

    public async Task<CredentialStatus> VerifyAsync(
        string url,
        Action<HttpRequestMessage> authorise,
        DateTimeOffset? expiresAt,
        CancellationToken cancellationToken)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, url);
        message.Options.Set(RetryHandler.IDEMPOTENT, true);
        authorise(message);
        using var response = await http.SendAsync(message, cancellationToken);
        return new CredentialStatus(response.IsSuccessStatusCode, StatusOf(response), expiresAt);
    }

    public string StatusOf(HttpResponseMessage response) =>
        $"{platform} returned {(int)response.StatusCode}";

    public async Task<PostResult> RejectedAsync(HttpResponseMessage response, string errorField, CancellationToken cancellationToken)
    {
        var status = StatusOf(response);
        var reason = ErrorTextFrom(await response.Content.ReadAsStringAsync(cancellationToken), errorField);
        return new PostResult.Rejected(reason is null ? status : $"{status}: {reason}");
    }

    public static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response, CancellationToken cancellationToken) =>
        await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);

    private Task<PostResult> ResultFromAsync(
        HttpResponseMessage response,
        Func<HttpResponseMessage, CancellationToken, Task<PostResult>> readAccepted,
        CancellationToken cancellationToken) =>
        response.StatusCode switch
        {
            HttpStatusCode.TooManyRequests => Task.FromResult<PostResult>(new PostResult.Rejected($"{platform} is rate limiting requests, so the post was not made")),
            >= HttpStatusCode.InternalServerError => Task.FromResult<PostResult>(new PostResult.Indeterminate($"{StatusOf(response)}, so the post may have been made")),
            _ => readAccepted(response, cancellationToken)
        };

    private static string? ErrorTextFrom(string body, string errorField)
    {
        try
        {
            using var json = JsonDocument.Parse(body);
            return json.RootElement is { ValueKind: JsonValueKind.Object } root
                   && root.TryGetProperty(errorField, out var text)
                   && text.ValueKind == JsonValueKind.String
                ? text.GetString()
                : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
