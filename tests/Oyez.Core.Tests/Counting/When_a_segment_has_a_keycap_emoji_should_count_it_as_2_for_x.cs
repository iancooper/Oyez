using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Counting;

public class XKeycapEmojiCountingTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Theory]
    [InlineData("1⃣")]
    [InlineData("1️⃣")]
    [InlineData("#️⃣")]
    [InlineData("*️⃣")]
    public async Task When_a_segment_has_a_keycap_emoji_should_count_it_as_2_for_x(string keycap)
    {
        //Arrange
        var text = $"{keycap} abcdefghi";
        _host.Announcements.Add(new AnnouncementDocument(
            _id,
            $"""
            title: Release
            channels: X
            ---
            {text}
            """,
            WriteAsMarkdown: null));

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        var post = Assert.Single(Assert.Single(result.Value.Renderings).Posts);
        Assert.Equal(12, post.Count);
    }

    public void Dispose() => _host.Dispose();
}
