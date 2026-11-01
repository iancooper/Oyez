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

using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests;

public static class PostRequests
{
    public static readonly string[] THREAD_TEXTS = ["Brighter 10.1 is out.", "It adds batching.", "Read more at the link."];

    public static PostRequest Build(ChannelId channel, int? limit, string? title, IEnumerable<string> texts, int index = 0, PostRef? replyTo = null)
    {
        var posts = texts.Select((text, i) => new Post(i, text, text.Length)).ToList();
        return new(
            new AnnouncementId("brighter-10-1"),
            new Rendering(channel, title, limit, posts),
            index,
            replyTo,
            "sha256:0123456789ab");
    }

    public static PostRequest Thread(ChannelId channel, int limit, int index = 0, PostRef? replyTo = null) =>
        Build(channel, limit, null, THREAD_TEXTS, index, replyTo);
}
