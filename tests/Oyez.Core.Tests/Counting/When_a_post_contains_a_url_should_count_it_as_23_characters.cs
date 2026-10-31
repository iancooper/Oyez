using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Counting;

public class UrlCountingTests : OyezApplicationTest
{
    [Theory]
    [InlineData("X", "https://github.com/BrighterCommand/Brighter/releases/tag/10.2.0")]
    [InlineData("X", "http://github.com/BrighterCommand/Brighter/releases/tag/10.2.0")]
    [InlineData("X", "https://t.co/a")]
    [InlineData("Mastodon", "https://github.com/BrighterCommand/Brighter/releases/tag/10.2.0")]
    [InlineData("Mastodon", "http://github.com/BrighterCommand/Brighter/releases/tag/10.2.0")]
    [InlineData("Mastodon", "https://t.co/a")]
    public async Task When_a_post_contains_a_url_should_count_it_as_23_characters(string channel, string url)
    {
        //Arrange
        const string TEXT_BEFORE_URL = "Released ";
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With(channel, $"{TEXT_BEFORE_URL}{url}"), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        var post = Assert.Single(Assert.Single(result.Value.Renderings).Posts);
        Assert.Equal(TEXT_BEFORE_URL.Length + 23, post.Count);
    }
}
