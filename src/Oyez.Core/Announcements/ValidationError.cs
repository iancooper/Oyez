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

namespace Oyez.Core.Announcements;

/// <summary>
/// A problem with an announcement, located as precisely as possible.
/// </summary>
/// <param name="Announcement">The <see cref="AnnouncementId"/> of the announcement with the problem.</param>
/// <param name="Message">What is wrong, and how to fix it where that's known.</param>
/// <param name="Channel">The <see cref="ChannelId"/> the problem affects, or <c>null</c> if it affects them all.</param>
/// <param name="Segment">The number of the segment with the problem, counting from 1, or <c>null</c> if it isn't in a segment.</param>
public sealed record ValidationError(
    AnnouncementId Announcement,
    string Message,
    ChannelId? Channel = null,
    int? Segment = null);
