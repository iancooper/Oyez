using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Previewing;

public class PreviewSegmentWhitespaceTests() : OyezApplicationTest(options => options.X.Limit = 30)
{
    [Fact]
    public async Task When_a_segment_has_surrounding_whitespace_should_trim_it_but_keep_its_inner_newlines()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(
            Id,
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
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        var x = result.Value.Renderings.Single(r => r.Channel == ChannelId.X);
        Assert.Equal("First line\nSecond line", x.Posts[0].Text);
    }
}
