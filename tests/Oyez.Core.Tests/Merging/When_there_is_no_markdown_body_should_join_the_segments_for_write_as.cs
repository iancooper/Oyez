using Oyez.Core.Announcements;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Merging;

public class WriteAsWithoutMarkdownTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_there_is_no_markdown_body_should_join_the_segments_for_write_as()
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(_id, AnnouncementText.With("writeas", "First segment", "Second segment"), WriteAsMarkdown: null));

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        var rendering = Assert.Single(result.Value.Renderings);
        Assert.Equal("Release", rendering.Title);
        Assert.Equal("First segment\n\nSecond segment", Assert.Single(rendering.Posts).Text);
    }

    public void Dispose() => _host.Dispose();
}
