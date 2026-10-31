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
/// Clears an indeterminate channel, after someone has checked the platform to see whether the post was made.
/// </summary>
/// <param name="announcement">The <see cref="AnnouncementId"/> of the announcement.</param>
/// <param name="channel">The <see cref="ChannelId"/> of the indeterminate channel.</param>
/// <param name="posted">The <see cref="PostRef"/> of the post that was made, or <c>null</c> if it was not made.</param>
public sealed class ResolveChannel(AnnouncementId announcement, ChannelId channel, PostRef? posted) : Command(Id.Random())
{
    /// <summary>Gets the announcement.</summary>
    /// <value>The <see cref="AnnouncementId"/> passed to the constructor.</value>
    public AnnouncementId Announcement { get; } = announcement;

    /// <summary>Gets the channel to resolve.</summary>
    /// <value>The <see cref="ChannelId"/> passed to the constructor.</value>
    public ChannelId Channel { get; } = channel;

    /// <summary>Gets the post that was made, if it was.</summary>
    /// <value>The <see cref="PostRef"/> passed to the constructor, or <c>null</c> if the post was not made.</value>
    public PostRef? Posted { get; } = posted;

    /// <summary>Gets or sets what the channel's state is now.</summary>
    /// <value>The new <see cref="ChannelState"/>, or the errors that stopped the channel being resolved, once the command has been handled.</value>
    public Result<ChannelState>? Result { get; set; }
}
