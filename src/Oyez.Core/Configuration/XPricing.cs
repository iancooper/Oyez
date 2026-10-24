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

namespace Oyez.Core.Configuration;

/// <summary>
/// What X charges for a post, in US dollars.
/// </summary>
public sealed class XPricing
{
    /// <summary>
    /// Gets or sets the price of a post that has no link.
    /// </summary>
    /// <value>Defaults to 0.015.</value>
    public decimal Post { get; set; } = 0.015m;

    /// <summary>
    /// Gets or sets the price of a post that contains a link, which includes a bare domain such as <c>example.com</c>.
    /// </summary>
    /// <value>Defaults to 0.20.</value>
    public decimal PostWithUrl { get; set; } = 0.20m;
}
