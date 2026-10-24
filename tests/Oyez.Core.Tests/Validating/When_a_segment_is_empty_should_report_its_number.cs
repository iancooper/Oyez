using Oyez.Core.Announcements;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidateEmptySegmentTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_a_segment_is_empty_should_report_its_number()
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(
            _id,
            """
            title: Release
            ---
            One
            ---

            ---
            Three
            """,
            WriteAsMarkdown: null));

        //Act
        var errors = await _host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([_id]), TestContext.Current.CancellationToken);

        //Assert
        var error = Assert.Single(errors);
        Assert.Equal(_id, error.Announcement);
        Assert.Equal(2, error.Segment);
    }

    public void Dispose() => _host.Dispose();
}
