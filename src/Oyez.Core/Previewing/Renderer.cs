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

internal sealed class Renderer(IEnumerable<IAmAChannelProfile> profiles)
{
    private const string SEGMENT_SEPARATOR = "\n\n";

    public IReadOnlyList<Rendering> RenderTargetedChannels(Announcement announcement) =>
        profiles
            .Where(profile => profile.IsEnabled && announcement.Channels.Contains(profile.Id))
            .Select(profile => Render(announcement, profile))
            .ToList();

    private static Rendering Render(Announcement announcement, IAmAChannelProfile profile) =>
        profile.Policy == RenderingPolicy.Document
            ? RenderDocument(announcement, profile)
            : RenderPosts(announcement, profile);

    private static Rendering RenderDocument(Announcement announcement, IAmAChannelProfile profile) =>
        new(profile.Id, announcement.Title, profile.Limit, [NewPost(profile, 0, announcement.WriteAsMarkdown ?? JoinSegments(announcement))]);

    private static Rendering RenderPosts(Announcement announcement, IAmAChannelProfile profile) =>
        new(profile.Id, null, profile.Limit, MergeSegments(announcement, profile).Select((text, index) => NewPost(profile, index, text)).ToList());

    private static string JoinSegments(Announcement announcement) =>
        string.Join(SEGMENT_SEPARATOR, announcement.Segments.Select(segment => segment.Text));

    private static List<string> MergeSegments(Announcement announcement, IAmAChannelProfile profile)
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
        var candidate = posts.Count == 0 ? text : posts[^1] + SEGMENT_SEPARATOR + text;
        if (posts.Count == 0 || !Fits(profile, candidate))
        {
            posts.Add(text);
        }
        else
        {
            posts[^1] = candidate;
        }
    }

    private static bool Fits(IAmAChannelProfile profile, string text) =>
        profile.Limit is not { } limit || profile.Counter.Count(profile.Prepare(text)) <= limit;

    private static Post NewPost(IAmAChannelProfile profile, int index, string text)
    {
        var prepared = profile.Prepare(text);
        return new Post(index, prepared, profile.Counter.Count(prepared));
    }
}
