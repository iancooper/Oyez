using Oyez.Core.Announcements;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidateSeveralProblemsTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_an_announcement_has_several_problems_should_report_them_all()
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(
            _id,
            """
            tags: release
            channels: bluesky
            ---
            One
            ---

            """,
            WriteAsMarkdown: null));

        //Act
        var errors = await _host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([_id]), TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal(4, errors.Count);
        Assert.All(errors, error => Assert.Equal(_id, error.Announcement));
        Assert.Contains(errors, error => error.Message.Contains("title", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(errors, error => error.Message.Contains("tags"));
        Assert.Contains(errors, error => error.Message.Contains("bluesky"));
        Assert.Contains(errors, error => error.Segment == 2);
    }

    public void Dispose() => _host.Dispose();
}
