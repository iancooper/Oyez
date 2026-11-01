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
/// The settings for reaching Write.as.
/// </summary>
public sealed class WriteAsOptions
{
    /// <summary>
    /// Gets or sets the secret Oyez presents to Write.as.
    /// </summary>
    /// <value>The access token.</value>
    public string AccessToken { get; set; } = "";

    /// <summary>
    /// Gets or sets the alias of the collection (the blog) that posts are made in.
    /// </summary>
    /// <value>The collection alias.</value>
    public string Collection { get; set; } = "";

    /// <summary>
    /// Gets or sets the address of Write.as's API.
    /// </summary>
    /// <value>Defaults to <c>https://write.as/api</c>.</value>
    public string ApiBase { get; set; } = "https://write.as/api";

    /// <summary>
    /// Gets or sets the address of the blog that shows the posts.
    /// </summary>
    /// <value>The blog's address, with no trailing slash.</value>
    public string BlogUrl { get; set; } = "";

    internal IEnumerable<string> Secrets() => [AccessToken];
}
