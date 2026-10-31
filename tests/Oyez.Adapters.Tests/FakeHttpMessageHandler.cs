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
using System.Text;

namespace Oyez.Adapters.Tests;

public sealed record SentRequest(HttpMethod Method, Uri Uri, IReadOnlyDictionary<string, string> Headers, string Body);

public sealed class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly List<SentRequest> _requests = [];
    private Func<SentRequest, HttpResponseMessage> _respond = _ => new HttpResponseMessage(HttpStatusCode.OK);

    public IReadOnlyList<SentRequest> Requests => _requests;

    public void RespondWith(HttpStatusCode status, string body) =>
        _respond = _ => new HttpResponseMessage(status) { Content = new StringContent(body, Encoding.UTF8, "application/json") };

    public void RespondWith(Func<int, HttpResponseMessage> respondToNth) =>
        _respond = _ => respondToNth(_requests.Count);

    public void Throw(Exception exception) =>
        _respond = _ => throw exception;

    public void RespondInSequence(params string[] okBodies) =>
        _respond = sent => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(okBodies[_requests.Count - 1], Encoding.UTF8, "application/json")
        };

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var body = request.Content is null ? "" : await request.Content.ReadAsStringAsync(cancellationToken);
        var headers = request.Headers.ToDictionary(header => header.Key, header => string.Join(",", header.Value));
        var sent = new SentRequest(request.Method, request.RequestUri!, headers, body);
        _requests.Add(sent);
        return _respond(sent);
    }
}
