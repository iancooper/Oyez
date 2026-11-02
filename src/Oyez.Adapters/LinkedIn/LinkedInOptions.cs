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
/// The settings for reaching LinkedIn.
/// </summary>
public sealed class LinkedInOptions
{
    /// <summary>
    /// Gets or sets the token that lets Oyez post as the member.
    /// </summary>
    /// <value>The access token.</value>
    public string AccessToken { get; set; } = "";

    /// <summary>
    /// Gets or sets the URN of the member Oyez posts as.
    /// </summary>
    /// <value>A person URN, such as <c>urn:li:person:8675309</c>.</value>
    public string PersonUrn { get; set; } = "";

    /// <summary>
    /// Gets or sets when the access token stops working.
    /// </summary>
    /// <value>The expiry time, or <c>null</c> if it is not known.</value>
    public DateTimeOffset? TokenExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the address of the LinkedIn API.
    /// </summary>
    /// <value>Defaults to <c>https://api.linkedin.com</c>.</value>
    public string ApiBase { get; set; } = "https://api.linkedin.com";

    /// <summary>
    /// Gets or sets the version of the LinkedIn API to ask for.
    /// </summary>
    /// <value>The version as <c>yyyyMM</c>, such as <c>202609</c>.</value>
    public string ApiVersion { get; set; } = "202609";

    internal IEnumerable<string> Secrets() => [AccessToken];
}
