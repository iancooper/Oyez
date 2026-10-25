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

namespace Oyez.Core.Publishing;

/// <summary>
/// What a publishing run did.
/// </summary>
/// <param name="Outcomes">A <see cref="PublishOutcome"/> for each channel the run tried to publish to.</param>
/// <param name="NotPublished">The due announcements the run skipped because they could not be read or failed validation.</param>
public sealed record PublishReport(IReadOnlyList<PublishOutcome> Outcomes, IReadOnlyList<UnpublishedAnnouncement> NotPublished)
{
    /// <summary>Gets the report as Markdown, for the GitHub Actions job summary.</summary>
    /// <value>Each channel's state, and the URLs of the posts this run made.</value>
    public string JobSummary => string.Join('\n', SummaryLines());

    private IEnumerable<string> SummaryLines() =>
        ["## Oyez publish", string.Empty, "| Announcement | Channel | State | Detail |", "| --- | --- | --- | --- |", .. Outcomes.Select(RowOf), .. NotPublishedLines()];

    private IEnumerable<string> NotPublishedLines() =>
        NotPublished.Select(unpublished => $"| {unpublished.Announcement} | - | Not published | {DescribeErrors(unpublished)} |");

    private static string DescribeErrors(UnpublishedAnnouncement unpublished) =>
        unpublished.Errors.Count == 1 ? unpublished.Errors[0].Message : $"{unpublished.Errors[0].Message} (and {unpublished.Errors.Count - 1} more)";

    private static string RowOf(PublishOutcome outcome) =>
        $"| {outcome.Announcement} | {outcome.Channel.DisplayName} | {outcome.State} | {DetailOf(outcome)} |";

    private static string DetailOf(PublishOutcome outcome) =>
        string.Join(' ', outcome.NewPosts.Select(post => post.Url).OfType<string>().Prepend(outcome.Reason).OfType<string>());
}
