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

namespace Oyez.Adapters;

/// <summary>
/// The settings for reaching X.
/// </summary>
public sealed class XOptions
{
    /// <summary>
    /// Gets or sets the key that identifies the Oyez application to X.
    /// </summary>
    /// <value>The consumer key (API key).</value>
    public string ConsumerKey { get; set; } = "";

    /// <summary>
    /// Gets or sets the secret that goes with the consumer key.
    /// </summary>
    /// <value>The consumer secret (API key secret).</value>
    public string ConsumerSecret { get; set; } = "";

    /// <summary>
    /// Gets or sets the token that lets Oyez post as the account.
    /// </summary>
    /// <value>The access token.</value>
    public string AccessToken { get; set; } = "";

    /// <summary>
    /// Gets or sets the secret that goes with the access token.
    /// </summary>
    /// <value>The access token secret.</value>
    public string AccessTokenSecret { get; set; } = "";

    /// <summary>
    /// Gets or sets the address of X's API.
    /// </summary>
    /// <value>Defaults to <c>https://api.x.com</c>.</value>
    public string ApiBase { get; set; } = "https://api.x.com";

    /// <summary>
    /// Gets or sets the handle of the account that posts, used to build post URLs.
    /// </summary>
    /// <value>The handle, without the leading <c>@</c>.</value>
    public string Handle { get; set; } = "";
}
