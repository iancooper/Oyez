using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Previewing;

public class PreviewSeparatorTests : IDisposable
{
    private readonly OyezApplicationHost _host = new(options => options.X.Limit = 5);
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_a_separator_line_has_trailing_whitespace_should_still_separate_segments()
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(
            _id,
            "title: Release\n--- \nOne\n---\t\nTwo",
            WriteAsMarkdown: null));

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        var x = result.Value.Renderings.Single(r => r.Channel == ChannelId.X);
        Assert.Equal(["One", "Two"], x.Posts.Select(p => p.Text));
    }

    public void Dispose() => _host.Dispose();
}
