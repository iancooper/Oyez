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

using Oyez.Core.Channels;
using Oyez.Core.Configuration;
using Oyez.Core.Counting;
using Oyez.Core.Previewing;

namespace Oyez.Core.Costing;

internal sealed class XCostEstimator(OyezOptions options)
{
    private readonly UrlMatcher _urls = UrlMatcher.ForX;

    public UsDollars? Estimate(IReadOnlyList<Rendering> renderings) =>
        renderings.FirstOrDefault(rendering => rendering.Channel == ChannelId.X) is { } x
            ? x.Posts.Select(PriceOf).Aggregate(new UsDollars(0m), (total, price) => total + price)
            : null;

    private UsDollars PriceOf(Post post) =>
        new(_urls.ContainsLink(post.Text) ? options.X.Pricing.PostWithUrl : options.X.Pricing.Post);
}
