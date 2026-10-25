using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Previewing;

public class PreviewSeparatorTests() : OyezApplicationTest(options => options.X.Limit = 5)
{
    [Fact]
    public async Task When_a_separator_line_has_trailing_whitespace_should_still_separate_segments()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(
            Id,
            "title: Release\n--- \nOne\n---\t\nTwo",
            WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        var x = result.Value.Renderings.Single(r => r.Channel == ChannelId.X);
        Assert.Equal(["One", "Two"], x.Posts.Select(p => p.Text));
    }
}
