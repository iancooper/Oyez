using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Previewing;

public class PreviewLineEndingTests : IDisposable
{
    private readonly OyezApplicationHost _host = new(options => options.X.Limit = 25);
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_an_announcement_has_crlf_line_endings_should_normalise_them()
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(
            _id,
            "title: Release\r\n---\r\nFirst line\r\nSecond line\r\n---\r\nThird\r\n",
            WriteAsMarkdown: "# Heading\r\n\r\nBody"));

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        var preview = result.Value;
        Assert.Equal("Release", preview.Title);
        var x = preview.Renderings.Single(r => r.Channel == ChannelId.X);
        Assert.Equal(["First line\nSecond line", "Third"], x.Posts.Select(p => p.Text));
        var writeAs = preview.Renderings.Single(r => r.Channel == ChannelId.WriteAs);
        Assert.Equal("# Heading\n\nBody", writeAs.Posts[0].Text);
    }

    public void Dispose() => _host.Dispose();
}
