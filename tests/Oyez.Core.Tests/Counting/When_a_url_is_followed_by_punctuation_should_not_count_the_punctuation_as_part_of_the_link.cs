using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Counting;

public class UrlPunctuationCountingTests : OyezApplicationTest
{
    [Theory]
    [InlineData("X", ".")]
    [InlineData("X", ",")]
    [InlineData("X", "!")]
    [InlineData("X", "?")]
    [InlineData("X", ")")]
    [InlineData("X", ".\"")]
    [InlineData("Mastodon", ".")]
    [InlineData("Mastodon", ",")]
    [InlineData("Mastodon", "!")]
    [InlineData("Mastodon", "?")]
    [InlineData("Mastodon", ")")]
    [InlineData("Mastodon", ".\"")]
    public async Task When_a_url_is_followed_by_punctuation_should_not_count_the_punctuation_as_part_of_the_link(
        string channel,
        string punctuation)
    {
        //Arrange
        const string TEXT_BEFORE_URL = "Read https://github.com/BrighterCommand/Brighter/releases";
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With(channel, $"{TEXT_BEFORE_URL}{punctuation}"), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        var post = Assert.Single(Assert.Single(result.Value.Renderings).Posts);
        Assert.Equal("Read ".Length + 23 + punctuation.Length, post.Count);
    }
}
