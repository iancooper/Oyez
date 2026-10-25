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

namespace Oyez.Core.Publishing;

/// <summary>
/// What has been published to one channel.
/// </summary>
/// <param name="State">The <see cref="ChannelState"/> of the channel.</param>
/// <param name="RenderingHash">The <see cref="Channels.RenderingHash"/> of the rendering that was being published.</param>
/// <param name="Posts">The <see cref="PublishedPost"/>s made so far.</param>
/// <param name="Reason">Why the channel is <see cref="ChannelState.Indeterminate"/> or <see cref="ChannelState.Failed"/>, if it is.</param>
public sealed record ChannelPublication(
    ChannelState State,
    RenderingHash RenderingHash,
    IReadOnlyList<PublishedPost> Posts,
    string? Reason = null)
{
    internal bool HasPostsToMake(Rendering rendering) =>
        State == ChannelState.Partial && Posts.Count < rendering.Posts.Count;

    internal bool IsEditedSince(Rendering rendering) =>
        Posts.Count > 0 && RenderingHash != rendering.Hash;

    internal ChannelPublication ResumeFor(Rendering rendering) =>
        this with { State = ChannelState.Partial, RenderingHash = rendering.Hash, Reason = null };

    internal ChannelPublication Append(PostRef posted, DateTimeOffset at, Rendering rendering)
    {
        var posts = Posts.Append(new PublishedPost(Posts.Count, posted, at)).ToList();
        var state = posts.Count == rendering.Posts.Count ? ChannelState.Complete : ChannelState.Partial;
        return this with { State = state, Posts = posts };
    }

    internal ChannelPublication Fail(string reason) =>
        this with { State = ChannelState.Failed, Reason = reason };

    internal ChannelPublication MarkIndeterminate(string reason) =>
        this with { State = ChannelState.Indeterminate, Reason = reason };

    internal static ChannelPublication Begin(Rendering rendering) =>
        new(ChannelState.Partial, rendering.Hash, []);
}
