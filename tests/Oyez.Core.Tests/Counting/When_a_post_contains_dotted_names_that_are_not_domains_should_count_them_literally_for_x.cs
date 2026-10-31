using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Counting;

public class DottedNameCountingForXTests : OyezApplicationTest
{
    [Theory]
    [InlineData("Paramore.Brighter")]
    [InlineData("v10.2.0")]
    [InlineData("e.g.")]
    [InlineData("file.cs")]
    public async Task When_a_post_contains_dotted_names_that_are_not_domains_should_count_them_literally_for_x(string name)
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("X", $"See {name} now"), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        var post = Assert.Single(Assert.Single(result.Value.Renderings).Posts);
        Assert.Equal("See ".Length + name.Length + " now".Length, post.Count);
    }
}
