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
using Oyez.Core.Previewing;

namespace Oyez.Core.Validating;

internal sealed class Validator(IEnumerable<IAmAChannelProfile> profiles, Renderer renderer)
{
    public IReadOnlyList<ValidationError> Validate(Announcement announcement) =>
        SegmentErrors(announcement)
            .Concat(renderer.RenderTargetedChannels(announcement).SelectMany(rendering => RenderingErrors(announcement, rendering)))
            .ToList();

    private IEnumerable<ValidationError> SegmentErrors(Announcement announcement)
    {
        var x = profiles.Single(profile => profile.Id == ChannelId.X);
        return x.Limit is { } limit
            ? announcement.Segments.Where(segment => x.Counter.Count(x.Prepare(segment.Text)) > limit).Select(segment => OverLimit(announcement, segment, limit))
            : [];
    }

    private static ValidationError OverLimit(Announcement announcement, Segment segment, int limit) =>
        new(announcement.Id, $"Segment {segment.Number} is over X's limit of {limit} characters. Shorten it, or split it into two segments.", ChannelId.X, segment.Number);

    private IEnumerable<ValidationError> RenderingErrors(Announcement announcement, Rendering rendering) =>
        profiles.Single(profile => profile.Id == rendering.Channel).Policy switch
        {
            RenderingPolicy.SinglePost => SinglePostErrors(announcement, rendering),
            RenderingPolicy.Document => DocumentErrors(announcement, rendering),
            _ => []
        };

    private static IEnumerable<ValidationError> SinglePostErrors(Announcement announcement, Rendering rendering) =>
        rendering.Posts.Count == 1
            ? []
            : [new ValidationError(announcement.Id, $"The {rendering.Channel} rendering doesn't fit in one post: it needs {rendering.Posts.Count}, and {rendering.Channel} takes one. Shorten the announcement, or remove {rendering.Channel} from 'channels'.", rendering.Channel)];

    private static IEnumerable<ValidationError> DocumentErrors(Announcement announcement, Rendering rendering)
    {
        if (string.IsNullOrWhiteSpace(rendering.Title))
            yield return new ValidationError(announcement.Id, $"The {rendering.Channel} rendering has no title.", rendering.Channel);
        if (rendering.Posts.All(post => string.IsNullOrWhiteSpace(post.Text)))
            yield return new ValidationError(announcement.Id, $"The {rendering.Channel} rendering has an empty body. Add text to the Markdown body, or remove it to build the body from the segments.", rendering.Channel);
    }
}
