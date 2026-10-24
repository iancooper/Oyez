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

namespace Oyez.Core.Announcements;

/// <summary>
/// Keeps announcements in memory. Use it in tests, or wherever announcements don't come from files.
/// </summary>
public sealed class InMemoryAnnouncementStore : IAmAnAnnouncementStore
{
    private readonly Dictionary<AnnouncementId, AnnouncementDocument> _documents = new();

    /// <summary>Adds an announcement, replacing any with the same ID.</summary>
    /// <param name="document">The <see cref="AnnouncementDocument"/> to add.</param>
    public void Add(AnnouncementDocument document) => _documents[document.Id] = document;

    /// <inheritdoc />
    public IReadOnlyList<AnnouncementId> List() => _documents.Keys.ToList();

    /// <inheritdoc />
    public AnnouncementDocument? Load(AnnouncementId id) => _documents.GetValueOrDefault(id);
}
