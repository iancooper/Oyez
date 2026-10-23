using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Previewing;

public class PreviewSegmentWhitespaceTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_a_segment_has_surrounding_whitespace_should_trim_it_but_keep_its_inner_newlines()
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(
            _id,
            """
            title: Release
            ---

              First line
            Second line

            ---
            Another segment
            """,
            WriteAsMarkdown: null));

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        var x = result.Value.Renderings.Single(r => r.Channel == ChannelId.X);
        Assert.Equal("First line\nSecond line", x.Posts[0].Text);
    }

    public void Dispose() => _host.Dispose();
}
