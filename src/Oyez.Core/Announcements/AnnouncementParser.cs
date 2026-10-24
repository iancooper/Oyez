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
using System.Text.RegularExpressions;

namespace Oyez.Core.Announcements;

// Reads announcement.txt: a header, then plain-text segments separated by lines that are exactly "---".
internal static partial class AnnouncementParser
{
    private const string SEPARATOR = "---";

    public static Result<Announcement> Parse(AnnouncementDocument document)
    {
        var blocks = SplitIntoBlocks(Normalise(document.Text));
        var header = AnnouncementHeader.Parse(document.Id, blocks[0]);
        var segments = ToSegments(blocks.Skip(1));
        var errors = IdErrors(document.Id).Concat(header.Errors).Concat(SegmentErrors(document.Id, segments)).ToList();

        return errors.Count == 0
            ? Result<Announcement>.Success(ToAnnouncement(document, header, segments))
            : Result<Announcement>.Failure(errors);
    }

    private static Announcement ToAnnouncement(AnnouncementDocument document, AnnouncementHeader header, IReadOnlyList<Segment> segments) =>
        new(
            document.Id,
            header.Title,
            header.PublishAt,
            header.Channels,
            segments,
            document.WriteAsMarkdown is null ? null : Normalise(document.WriteAsMarkdown));

    private static IEnumerable<ValidationError> IdErrors(AnnouncementId id) =>
        FolderName().IsMatch(id.Value)
            ? []
            : [new ValidationError(id, $"'{id}' isn't a valid announcement folder name. Use lower-case letters, digits and hyphens, starting with a letter or digit, such as 2026-11-03-brighter-10-3-0.")];

    private static IEnumerable<ValidationError> SegmentErrors(AnnouncementId id, IReadOnlyList<Segment> segments) =>
        segments.Count == 0
            ? [new ValidationError(id, "The announcement has no segments. Add a '---' line after the header, then the text of each segment.")]
            : segments.Where(segment => segment.Text.Length == 0).Select(segment => EmptySegment(id, segment));

    private static ValidationError EmptySegment(AnnouncementId id, Segment segment) =>
        new(id, $"Segment {segment.Number} is empty. Add its text, or remove the extra '---' line.", Segment: segment.Number);

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
        {
            blocks.Add([]);
        }
        else
        {
            blocks[^1].Add(line);
        }
    }

    private static bool IsSeparator(string line) => line.TrimEnd() == SEPARATOR;

    private static List<Segment> ToSegments(IEnumerable<List<string>> blocks) =>
        blocks.Select((lines, index) => new Segment(index + 1, string.Join('\n', lines).Trim())).ToList();

    [GeneratedRegex("^[a-z0-9][a-z0-9-]*$")]
    private static partial Regex FolderName();
}
