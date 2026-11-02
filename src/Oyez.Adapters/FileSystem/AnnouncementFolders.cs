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

using System.Diagnostics.CodeAnalysis;
using Oyez.Core.Announcements;

namespace Oyez.Adapters.FileSystem;

internal sealed class AnnouncementFolders(string root)
{
    public string Root => root;

    public bool TryFolderOf(AnnouncementId id, [NotNullWhen(true)] out string? folder)
    {
        var rootPath = Path.TrimEndingDirectorySeparator(Path.GetFullPath(root));
        var folderPath = Path.GetFullPath(Path.Combine(rootPath, id.Value));
        var inside = folderPath.StartsWith(rootPath + Path.DirectorySeparatorChar, StringComparison.Ordinal);
        folder = inside ? Path.Combine(root, id.Value) : null;
        return inside;
    }

    public string FolderOf(AnnouncementId id) =>
        TryFolderOf(id, out var folder)
            ? folder
            : throw new ArgumentException($"Announcement '{id.Value}' is outside the announcements folder.", nameof(id));

    public string FileOf(AnnouncementId id, string name) => Path.Combine(FolderOf(id), name);
}
