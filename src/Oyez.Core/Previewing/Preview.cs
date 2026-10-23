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

namespace Oyez.Core.Previewing;

/// <summary>
/// How an announcement will look on each channel it targets.
/// </summary>
/// <param name="Id">The <see cref="AnnouncementId"/> of the announcement.</param>
/// <param name="Title">The announcement's title.</param>
/// <param name="PublishAt">When the announcement is due, in UTC, or <c>null</c> to publish it as soon as possible.</param>
/// <param name="Renderings">One <see cref="Rendering"/> per targeted channel, in the order of <see cref="ChannelId.All"/>.</param>
public sealed record Preview(
    AnnouncementId Id,
    string Title,
    DateTimeOffset? PublishAt,
    IReadOnlyList<Rendering> Renderings);
