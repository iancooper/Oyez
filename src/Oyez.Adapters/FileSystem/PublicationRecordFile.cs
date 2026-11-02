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
using Oyez.Core.Publishing;

namespace Oyez.Adapters.FileSystem;

internal sealed record PublicationRecordFile(Dictionary<string, PublicationRecordFile.ChannelFile> Channels)
{
    internal static PublicationRecordFile From(PublicationRecord record) =>
        new(record.Channels.ToDictionary(pair => pair.Key.Value, pair => ChannelFile.From(pair.Value)));

    internal PublicationRecord ToRecord() =>
        new(Channels.ToDictionary(pair => ChannelIdOf(pair.Key), pair => pair.Value.ToPublication()));

    private static ChannelId ChannelIdOf(string name) =>
        ChannelId.TryParse(name, out var channel) ? channel : throw new InvalidDataException($"Unknown channel '{name}' in a publication record.");

    internal sealed record ChannelFile(ChannelState State, string RenderingHash, List<PostFile> Posts, string? Reason)
    {
        internal ChannelPublication ToPublication() =>
            new(State, new RenderingHash(RenderingHash), Posts.Select(post => post.ToPost()).ToList(), Reason);

        internal static ChannelFile From(ChannelPublication publication) =>
            new(publication.State, publication.RenderingHash.Value, publication.Posts.Select(PostFile.From).ToList(), publication.Reason);
    }

    internal sealed record PostFile(int Index, string Id, string? Url, DateTimeOffset At)
    {
        internal PublishedPost ToPost() => new(Index, new PostRef(Id, Url), At);

        internal static PostFile From(PublishedPost post) =>
            new(post.Index, post.Ref.Id, post.Ref.Url, post.At);
    }
}
