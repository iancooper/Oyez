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

namespace Oyez.Core.Publishing;

/// <summary>
/// What publishing did for one channel of one announcement.
/// </summary>
/// <param name="Announcement">The <see cref="AnnouncementId"/> of the announcement.</param>
/// <param name="Channel">The <see cref="ChannelId"/> that was published to.</param>
/// <param name="State">The <see cref="ChannelState"/> the channel reached.</param>
/// <param name="Reason">Why the channel is <see cref="ChannelState.Failed"/> or <see cref="ChannelState.Indeterminate"/>, if it is. For an indeterminate channel that was skipped, this is the reason it was marked, and is the warning to show.</param>
public sealed record PublishOutcome(AnnouncementId Announcement, ChannelId Channel, ChannelState State, string? Reason = null);
