using Oyez.Core.Announcements;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Previewing;

public class PreviewPublishTimeTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Theory]
    [InlineData("2026-11-03T09:00Z")]
    [InlineData("2026-11-03T09:00:00Z")]
    [InlineData("2026-11-03T10:00+01:00")]
    [InlineData("2026-11-03T04:30:00-04:30")]
    public async Task When_an_announcement_has_a_publish_time_should_preview_it_in_utc(string publish)
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(
            _id,
            $"""
            title: Release
            publish: {publish}
            ---
            Segment
            """,
            WriteAsMarkdown: null));

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        var publishAt = result.Value.PublishAt;
        Assert.Equal(new DateTimeOffset(2026, 11, 3, 9, 0, 0, TimeSpan.Zero), publishAt);
        Assert.Equal(TimeSpan.Zero, publishAt!.Value.Offset);
    }

    public void Dispose() => _host.Dispose();
}
