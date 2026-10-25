using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Counting;

public class BareDomainCountingForXTests : OyezApplicationTest
{
    [Theory]
    [InlineData("brighter.io")]
    [InlineData("asp.net")]
    [InlineData("www.example.com/path")]
    [InlineData("Brighter.IO")]
    [InlineData("brighter.io.")]
    public async Task When_a_post_contains_a_bare_domain_should_count_it_as_23_characters_for_x(string domain)
    {
        //Arrange
        var trailingDot = domain.EndsWith('.') ? 1 : 0;
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("X", $"Visit {domain} now"), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        var post = Assert.Single(Assert.Single(result.Value.Renderings).Posts);
        Assert.Equal("Visit ".Length + 23 + trailingDot + " now".Length, post.Count);
    }
}
