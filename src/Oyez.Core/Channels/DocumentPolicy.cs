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
using Oyez.Core.Validating;

namespace Oyez.Core.Channels;

internal sealed class DocumentPolicy : IAmARenderingPolicy
{
    public Rendering Render(Announcement announcement, IAmAChannelProfile profile) =>
        new(profile.Id, announcement.Title, profile.Limit, [profile.PostFor(announcement.WriteAsMarkdown ?? SegmentMerger.Join(announcement), 0)]);

    public IEnumerable<ValidationError> Errors(Announcement announcement, IAmAChannelProfile profile, Rendering rendering) =>
        rendering.Posts.All(post => string.IsNullOrWhiteSpace(post.Text))
            ? [new ValidationError(announcement.Id, $"The {rendering.Channel} rendering has an empty body. Add text to the Markdown body, or remove it to build the body from the segments.", rendering.Channel)]
            : [];
}
