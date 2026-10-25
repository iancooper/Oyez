using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Previewing;

public class PreviewLineEndingTests() : OyezApplicationTest(options => options.X.Limit = 25)
{
    [Fact]
    public async Task When_an_announcement_has_crlf_line_endings_should_normalise_them()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(
            Id,
            "title: Release\r\n---\r\nFirst line\r\nSecond line\r\n---\r\nThird\r\n",
            WriteAsMarkdown: "# Heading\r\n\r\nBody"));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        var preview = result.Value;
        Assert.Equal("Release", preview.Title);
        var x = preview.Renderings.Single(r => r.Channel == ChannelId.X);
        Assert.Equal(["First line\nSecond line", "Third"], x.Posts.Select(p => p.Text));
        var writeAs = preview.Renderings.Single(r => r.Channel == ChannelId.WriteAs);
        Assert.Equal("# Heading\n\nBody", writeAs.Posts[0].Text);
    }
}
