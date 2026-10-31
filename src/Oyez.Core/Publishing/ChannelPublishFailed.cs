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
using Paramore.Brighter;

namespace Oyez.Core.Publishing;

/// <summary>
/// Raised after a channel stopped because a post was rejected, may have been made, or could not be retried, and the publication record was saved.
/// </summary>
/// <param name="announcement">The <see cref="AnnouncementId"/> of the announcement.</param>
/// <param name="channel">The <see cref="ChannelId"/> that stopped.</param>
/// <param name="state">Either <see cref="ChannelState.Failed"/> or <see cref="ChannelState.Indeterminate"/>.</param>
/// <param name="reason">Why the channel stopped.</param>
public sealed class ChannelPublishFailed(AnnouncementId announcement, ChannelId channel, ChannelState state, string reason) : Event(Id.Random())
{
    /// <summary>Gets the announcement that was being published.</summary>
    /// <value>The <see cref="AnnouncementId"/>.</value>
    public AnnouncementId Announcement { get; } = announcement;

    /// <summary>Gets the channel that stopped.</summary>
    /// <value>The <see cref="ChannelId"/>.</value>
    public ChannelId Channel { get; } = channel;

    /// <summary>Gets where the channel got to.</summary>
    /// <value>Either <see cref="ChannelState.Failed"/> or <see cref="ChannelState.Indeterminate"/>.</value>
    public ChannelState State { get; } = state;

    /// <summary>Gets why the channel stopped.</summary>
    /// <value>The reason the platform or Oyez gave.</value>
    public string Reason { get; } = reason;
}
