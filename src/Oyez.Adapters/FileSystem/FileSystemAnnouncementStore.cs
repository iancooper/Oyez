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

namespace Oyez.Adapters.FileSystem;

internal sealed class FileSystemAnnouncementStore(AnnouncementFolders folders) : IAmAnAnnouncementStore
{
    private const string ANNOUNCEMENT_FILE = "announcement.txt";
    private const string WRITEAS_FILE = "writeas.md";

    public IReadOnlyList<AnnouncementId> List() =>
        !Directory.Exists(folders.Root)
            ? []
            : Directory.EnumerateDirectories(folders.Root)
                .Select(Path.GetFileName)
                .OfType<string>()
                .Where(name => !name.StartsWith('.'))
                .Where(name => File.Exists(folders.FileOf(new AnnouncementId(name), ANNOUNCEMENT_FILE)))
                .Order(StringComparer.Ordinal)
                .Select(name => new AnnouncementId(name))
                .ToList();

    public AnnouncementDocument? Load(AnnouncementId id)
    {
        if (!folders.Contains(id))
        {
            return null;
        }

        var textPath = folders.FileOf(id, ANNOUNCEMENT_FILE);
        return File.Exists(textPath)
            ? new AnnouncementDocument(id, File.ReadAllText(textPath), ReadMarkdown(id))
            : null;
    }

    private string? ReadMarkdown(AnnouncementId id)
    {
        var markdownPath = folders.FileOf(id, WRITEAS_FILE);
        return File.Exists(markdownPath) ? File.ReadAllText(markdownPath) : null;
    }
}
