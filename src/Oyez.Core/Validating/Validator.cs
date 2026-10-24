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

namespace Oyez.Core.Validating;

internal sealed class Validator(IEnumerable<IAmAChannelProfile> profiles, Renderer renderer)
{
    private readonly IAmAChannelProfile _x = profiles.Single(profile => profile.Id == ChannelId.X);

    public IReadOnlyList<ValidationError> Validate(Announcement announcement) =>
        SegmentErrors(announcement)
            .Concat(renderer.TargetedProfiles(announcement).SelectMany(profile => RenderingErrors(announcement, profile)))
            .ToList();

    private IEnumerable<ValidationError> SegmentErrors(Announcement announcement) =>
        _x.Limit is { } limit
            ? announcement.Segments.Where(segment => _x.Measure(segment.Text) > limit).Select(segment => OverLimit(announcement, segment, limit))
            : [];

    private static ValidationError OverLimit(Announcement announcement, Segment segment, int limit) =>
        new(announcement.Id, $"Segment {segment.Number} is over X's limit of {limit} characters. Shorten it, or split it into two segments.", ChannelId.X, segment.Number);

    private IEnumerable<ValidationError> RenderingErrors(Announcement announcement, IAmAChannelProfile profile) =>
        profile.Policy.Errors(announcement, renderer.Render(announcement, profile));
}
