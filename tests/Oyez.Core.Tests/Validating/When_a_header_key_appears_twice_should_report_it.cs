using Oyez.Core.Announcements;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidateDuplicateHeaderKeyTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_a_header_key_appears_twice_should_report_it()
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(
            _id,
            """
            title: Release
            Title: Another release
            ---
            Segment
            """,
            WriteAsMarkdown: null));

        //Act
        var errors = await _host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([_id]), TestContext.Current.CancellationToken);

        //Assert
        var error = Assert.Single(errors);
        Assert.Equal(_id, error.Announcement);
        Assert.Contains("Title", error.Message);
        Assert.Contains("Remove", error.Message);
    }

    public void Dispose() => _host.Dispose();
}
