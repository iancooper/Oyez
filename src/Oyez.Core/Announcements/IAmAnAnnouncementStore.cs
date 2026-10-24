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
/// Reads announcements from where they are kept, such as the <c>announcements/</c> folder.
/// </summary>
/// <remarks>
/// A store only returns the raw <see cref="AnnouncementDocument"/>. Oyez parses and validates it.
/// </remarks>
public interface IAmAnAnnouncementStore
{
    /// <summary>Lists every announcement in the store.</summary>
    /// <returns>The <see cref="AnnouncementId"/> of each announcement.</returns>
    IReadOnlyList<AnnouncementId> List();

    /// <summary>Reads one announcement's files.</summary>
    /// <param name="id">The <see cref="AnnouncementId"/> of the announcement to read.</param>
    /// <returns>The <see cref="AnnouncementDocument"/>, or <c>null</c> if there is no such announcement.</returns>
    AnnouncementDocument? Load(AnnouncementId id);
}
