using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Counting;

public class BareDomainCountingForMastodonTests : OyezApplicationTest
{
    [Theory]
    [InlineData("brighter.io")]
    [InlineData("asp.net")]
    [InlineData("www.example.com/path")]
    public async Task When_a_post_contains_a_bare_domain_should_count_it_literally_for_mastodon(string domain)
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("Mastodon", $"Visit {domain} now"), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        var post = Assert.Single(Assert.Single(result.Value.Renderings).Posts);
        Assert.Equal("Visit ".Length + domain.Length + " now".Length, post.Count);
    }
}
