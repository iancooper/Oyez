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

using Microsoft.Extensions.Logging;

namespace Oyez.Adapters;

internal sealed class RedactingLoggingHandler(ILogger<RedactingLoggingHandler> logger, SecretRedactor redactor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await SendLoggingFailureAsync(request, cancellationToken);
        var target = TargetOf(request);
        logger.LogInformation("{Method} {Target} returned {Status}", request.Method, target, (int)response.StatusCode);
        if (IsClientError(response))
        {
            await LogBodyAsync(request, target, response, cancellationToken);
        }
        return response;
    }

    private async Task<HttpResponseMessage> SendLoggingFailureAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            return await base.SendAsync(request, cancellationToken);
        }
        catch (Exception failure)
        {
            logger.LogError("{Method} {Target} failed: {Reason}", request.Method, TargetOf(request), redactor.Redact(failure.Message));
            throw;
        }
    }

    private async Task LogBodyAsync(HttpRequestMessage request, string target, HttpResponseMessage response, CancellationToken cancellationToken)
    {
        await response.Content.LoadIntoBufferAsync(cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        logger.LogWarning("{Method} {Target} was refused with {Body}", request.Method, target, redactor.Redact(body));
    }

    private static string TargetOf(HttpRequestMessage request) =>
        $"{request.RequestUri!.Host}{request.RequestUri.AbsolutePath}";

    private static bool IsClientError(HttpResponseMessage response) =>
        (int)response.StatusCode is >= 400 and < 500;
}
