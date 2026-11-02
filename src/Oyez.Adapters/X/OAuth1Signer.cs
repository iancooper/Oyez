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

using System.Security.Cryptography;
using System.Text;

namespace Oyez.Adapters;

internal sealed class OAuth1Signer(XOptions options, TimeProvider clock, IAmANonceSource nonces)
{
    private const string SIGNATURE_METHOD = "HMAC-SHA1";
    private const string VERSION = "1.0";
    private const string UNRESERVED = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~";

    public string AuthorizationFor(HttpMethod method, Uri uri)
    {
        var oauth = OAuthParameters();
        var signature = SignatureFor(method, uri, oauth);
        var header = oauth.Append(new("oauth_signature", signature)).OrderBy(pair => pair.Key, StringComparer.Ordinal);
        return "OAuth " + string.Join(", ", header.Select(pair => $"{pair.Key}=\"{Encode(pair.Value)}\""));
    }

    private List<KeyValuePair<string, string>> OAuthParameters() =>
    [
        new("oauth_consumer_key", options.ConsumerKey),
        new("oauth_nonce", nonces.Next()),
        new("oauth_signature_method", SIGNATURE_METHOD),
        new("oauth_timestamp", clock.GetUtcNow().ToUnixTimeSeconds().ToString()),
        new("oauth_token", options.AccessToken),
        new("oauth_version", VERSION)
    ];

    private string SignatureFor(HttpMethod method, Uri uri, IEnumerable<KeyValuePair<string, string>> oauth)
    {
        var baseString = $"{method.Method}&{Encode(BaseUrlOf(uri))}&{Encode(ParameterString(uri, oauth))}";
        var key = Encoding.ASCII.GetBytes($"{Encode(options.ConsumerSecret)}&{Encode(options.AccessTokenSecret)}");
        return Convert.ToBase64String(HMACSHA1.HashData(key, Encoding.ASCII.GetBytes(baseString)));
    }

    private static string BaseUrlOf(Uri uri) => uri.GetLeftPart(UriPartial.Path);

    private static string ParameterString(Uri uri, IEnumerable<KeyValuePair<string, string>> oauth)
    {
        var encoded = oauth.Concat(QueryParametersOf(uri)).Select(pair => (Key: Encode(pair.Key), Value: Encode(pair.Value)));
        return string.Join("&", encoded.OrderBy(pair => pair.Key, StringComparer.Ordinal).ThenBy(pair => pair.Value, StringComparer.Ordinal).Select(pair => $"{pair.Key}={pair.Value}"));
    }

    private static IEnumerable<KeyValuePair<string, string>> QueryParametersOf(Uri uri) =>
        uri.Query.TrimStart('?')
            .Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(part => part.Split('=', 2))
            .Select(part => new KeyValuePair<string, string>(Uri.UnescapeDataString(part[0]), part.Length > 1 ? Uri.UnescapeDataString(part[1]) : ""));

    private static string Encode(string value)
    {
        var builder = new StringBuilder();
        foreach (var octet in Encoding.UTF8.GetBytes(value))
        {
            builder.Append(UNRESERVED.Contains((char)octet) ? ((char)octet).ToString() : $"%{octet:X2}");
        }
        return builder.ToString();
    }
}
