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

using System.Text;

namespace Oyez.Core.Announcements;

// Reads announcement.txt: a header, then plain-text segments separated by lines that are exactly "---".
internal static class AnnouncementParser
{
    private const string SEPARATOR = "---";

    public static Result<Announcement> Parse(AnnouncementDocument document)
    {
        var blocks = SplitIntoBlocks(Normalise(document.Text));
        var header = AnnouncementHeader.Parse(blocks[0]);

        return Result<Announcement>.Success(new Announcement(
            document.Id,
            header.Title,
            header.PublishAt,
            header.Channels,
            ToSegments(blocks.Skip(1)),
            document.WriteAsMarkdown is null ? null : Normalise(document.WriteAsMarkdown)));
    }

    private static string Normalise(string text) =>
        text.Replace("\r\n", "\n").Replace('\r', '\n').Normalize(NormalizationForm.FormC);

    private static List<List<string>> SplitIntoBlocks(string text)
    {
        var blocks = new List<List<string>> { new() };
        foreach (var line in text.Split('\n'))
        {
            AddToBlocks(blocks, line);
        }

        return blocks;
    }

    private static void AddToBlocks(List<List<string>> blocks, string line)
    {
        if (IsSeparator(line))
            blocks.Add([]);
        else
            blocks[^1].Add(line);
    }

    private static bool IsSeparator(string line) => line.TrimEnd() == SEPARATOR;

    private static List<Segment> ToSegments(IEnumerable<List<string>> blocks) =>
        blocks.Select((lines, index) => new Segment(index + 1, string.Join('\n', lines).Trim())).ToList();
}
