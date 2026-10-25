using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Preparing;

public class LinkedInEscapingTests : OyezApplicationTest
{
    [Theory]
    [InlineData('\\')]
    [InlineData('|')]
    [InlineData('{')]
    [InlineData('}')]
    [InlineData('@')]
    [InlineData('[')]
    [InlineData(']')]
    [InlineData('(')]
    [InlineData(')')]
    [InlineData('<')]
    [InlineData('>')]
    [InlineData('#')]
    [InlineData('*')]
    [InlineData('_')]
    [InlineData('~')]
    public async Task When_a_post_has_reserved_characters_should_escape_them_for_linkedin(char reserved)
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("LinkedIn", $"Fast {reserved} release"), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        var post = Assert.Single(Assert.Single(result.Value.Renderings).Posts);
        Assert.Equal($"Fast \\{reserved} release", post.Text);
    }
}
