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

internal static class SegmentMerger
{
    private const string SEGMENT_SEPARATOR = "\n\n";

    public static Rendering Render(Announcement announcement, IAmAChannelProfile profile) =>
        new(profile.Id, null, profile.Limit, Merge(announcement, profile).Select(profile.PostFor).ToList());

    public static string Join(Announcement announcement) =>
        string.Join(SEGMENT_SEPARATOR, announcement.Segments.Select(segment => segment.Text));

    private static List<string> Merge(Announcement announcement, IAmAChannelProfile profile)
    {
        var posts = new List<string>();
        foreach (var segment in announcement.Segments)
        {
            Append(posts, segment.Text, profile);
        }

        return posts;
    }

    private static void Append(List<string> posts, string text, IAmAChannelProfile profile)
    {
        var candidate = posts.Count == 0 ? null : posts[^1] + SEGMENT_SEPARATOR + text;
        if (candidate is not null && profile.Fits(candidate))
        {
            posts[^1] = candidate;
        }
        else
        {
            posts.Add(text);
        }
    }
}
