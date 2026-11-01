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
using System.Net.Sockets;
using System.Runtime.ExceptionServices;

namespace Oyez.Adapters;

internal sealed class RetryHandler(TimeProvider clock) : DelegatingHandler
{
    public static readonly HttpRequestOptionsKey<bool> IDEMPOTENT = new("Oyez.Idempotent");

    private const int MAX_RETRIES = 3;
    private static readonly TimeSpan MAX_RETRY_AFTER = TimeSpan.FromSeconds(60);

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var retries = 0;
        while (true)
        {
            var (response, failure) = await TryAsync(request, cancellationToken);
            var delay = DelayBeforeRetry(request, response, failure, retries);
            if (delay is null)
            {
                return response ?? Rethrow(failure!);
            }
            response?.Dispose();
            retries++;
            await Task.Delay(delay.Value, clock, cancellationToken);
        }
    }

    private async Task<(HttpResponseMessage? Response, Exception? Failure)> TryAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            return (await base.SendAsync(request, cancellationToken), null);
        }
        catch (Exception failure) when (failure is HttpRequestException or TaskCanceledException && !cancellationToken.IsCancellationRequested)
        {
            return (null, failure);
        }
    }

    private static TimeSpan? DelayBeforeRetry(HttpRequestMessage request, HttpResponseMessage? response, Exception? failure, int retries)
    {
        if (retries >= MAX_RETRIES)
        {
            return null;
        }
        var idempotent = request.Options.TryGetValue(IDEMPOTENT, out var marked) && marked;
        return response is null ? DelayAfterFailure(failure!, idempotent, retries) : DelayAfterResponse(response, idempotent, retries);
    }

    private static TimeSpan? DelayAfterFailure(Exception failure, bool idempotent, int retries) =>
        IsSafeToRepeat(failure, idempotent) ? Backoff(retries) : null;

    private static bool IsSafeToRepeat(Exception failure, bool idempotent) =>
        idempotent || (failure is HttpRequestException { InnerException: SocketException socket } && IsBeforeSend(socket));

    private static TimeSpan? DelayAfterResponse(HttpResponseMessage response, bool idempotent, int retries)
    {
        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            var retryAfter = response.Headers.RetryAfter?.Delta;
            return retryAfter <= MAX_RETRY_AFTER ? retryAfter : null;
        }
        return idempotent && response.StatusCode >= HttpStatusCode.InternalServerError ? Backoff(retries) : null;
    }

    private static TimeSpan Backoff(int retries) => TimeSpan.FromSeconds(Math.Pow(2, retries));

    private static bool IsBeforeSend(SocketException socket) =>
        socket.SocketErrorCode is SocketError.ConnectionRefused or SocketError.HostNotFound;

    private static HttpResponseMessage Rethrow(Exception failure)
    {
        ExceptionDispatchInfo.Capture(failure).Throw();
        throw failure;
    }
}
