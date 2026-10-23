using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Previewing;

public class PreviewEveryChannelTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_previewing_an_announcement_should_show_its_title_segments_and_markdown_for_every_channel()
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(
            _id,
            """
            title: Brighter 10.3.0 released
            ---
            Brighter 10.3.0 is out!
            ---
            Thanks to everyone who contributed.
            """,
            WriteAsMarkdown: "# Brighter 10.3.0\n\nRelease notes."));

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        Assert.True(result.IsSuccess);
        var preview = result.Value;
        Assert.Equal(_id, preview.Id);
        Assert.Equal("Brighter 10.3.0 released", preview.Title);
        Assert.Null(preview.PublishAt);
        Assert.Equal(ChannelId.All, preview.Renderings.Select(r => r.Channel));

        var postRenderings = preview.Renderings.Where(r => r.Channel != ChannelId.WriteAs);
        Assert.All(postRenderings, rendering =>
            Assert.Equal(["Brighter 10.3.0 is out!", "Thanks to everyone who contributed."], rendering.Posts.Select(p => p.Text)));

        var writeAs = preview.Renderings.Single(r => r.Channel == ChannelId.WriteAs);
        Assert.Equal("Brighter 10.3.0 released", writeAs.Title);
        Assert.Equal("# Brighter 10.3.0\n\nRelease notes.", Assert.Single(writeAs.Posts).Text);
    }

    public void Dispose() => _host.Dispose();
}
