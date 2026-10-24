using Oyez.Core.Announcements;
using Oyez.Core.Previewing;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Previewing;

public class PreviewMalformedAnnouncementTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_previewing_a_malformed_announcement_should_return_its_validation_errors()
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(
            _id,
            """
            tags: release
            ---
            Segment
            """,
            WriteAsMarkdown: null));
        var validationErrors = await _host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([_id]), TestContext.Current.CancellationToken);

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
        Assert.Equal(validationErrors, result.Errors);
    }

    public void Dispose() => _host.Dispose();
}
