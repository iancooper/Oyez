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
/// Asks a publisher to make one post.
/// </summary>
/// <param name="Announcement">The <see cref="AnnouncementId"/> of the announcement being published.</param>
/// <param name="Rendering">The <see cref="Rendering"/> the post belongs to.</param>
/// <param name="Index">The position of the post in the rendering, from zero.</param>
/// <param name="ReplyTo">The post this one replies to, or <c>null</c> for the first post.</param>
/// <param name="IdempotencyKey">A key that is the same every time the same post is attempted, for platforms that can use it to avoid duplicates.</param>
public sealed record PostRequest(AnnouncementId Announcement, Rendering Rendering, int Index, PostRef? ReplyTo, string IdempotencyKey);
