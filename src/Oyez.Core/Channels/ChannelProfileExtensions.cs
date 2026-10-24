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

namespace Oyez.Core.Channels;

internal static class ChannelProfileExtensions
{
    public static Rendering Render(this IAmAChannelProfile profile, Announcement announcement) =>
        profile.Policy.Render(announcement, profile);

    public static IEnumerable<ValidationError> Errors(this IAmAChannelProfile profile, Announcement announcement) =>
        profile.Policy.Errors(announcement, profile);

    public static int Measure(this IAmAChannelProfile profile, string text) =>
        profile.Counter.Count(profile.Prepare(text));

    public static bool Fits(this IAmAChannelProfile profile, string text) =>
        profile.Limit is not { } limit || profile.Measure(text) <= limit;

    public static Post PostFor(this IAmAChannelProfile profile, string text, int index)
    {
        var prepared = profile.Prepare(text);
        return new Post(index, prepared, profile.Counter.Count(prepared));
    }

    public static IEnumerable<ValidationError> OverLimitSegments(this IAmAChannelProfile profile, Announcement announcement) =>
        profile.Limit is { } limit
            ? announcement.Segments.Where(segment => profile.Measure(segment.Text) > limit).Select(segment => OverLimit(announcement, segment, profile.Id, limit))
            : [];

    private static ValidationError OverLimit(Announcement announcement, Segment segment, ChannelId channel, int limit) =>
        new(announcement.Id, $"Segment {segment.Number} is over {channel}'s limit of {limit} characters. Shorten it, or split it into two segments.", channel, segment.Number);
}
