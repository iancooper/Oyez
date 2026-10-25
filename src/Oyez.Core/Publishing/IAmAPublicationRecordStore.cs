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

namespace Oyez.Core.Publishing;

/// <summary>
/// Keeps the <see cref="PublicationRecord"/> of each announcement, such as in its <c>published.json</c> file.
/// </summary>
public interface IAmAPublicationRecordStore
{
    /// <summary>Reads an announcement's record.</summary>
    /// <param name="id">The <see cref="AnnouncementId"/> of the announcement.</param>
    /// <returns>The <see cref="PublicationRecord"/>, or <see cref="PublicationRecord.Empty"/> if there is none.</returns>
    PublicationRecord Load(AnnouncementId id);

    /// <summary>Writes an announcement's record, replacing any earlier one.</summary>
    /// <param name="id">The <see cref="AnnouncementId"/> of the announcement.</param>
    /// <param name="record">The <see cref="PublicationRecord"/> to keep.</param>
    void Save(AnnouncementId id, PublicationRecord record);
}
