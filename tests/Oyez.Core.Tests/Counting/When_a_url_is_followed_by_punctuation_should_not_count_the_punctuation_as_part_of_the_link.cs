using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Counting;

public class UrlPunctuationCountingTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

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
        _host.Announcements.Add(new AnnouncementDocument(
            _id,
            $"""
            title: Release
            channels: {channel}
            ---
            {TEXT_BEFORE_URL}{punctuation}
            """,
            WriteAsMarkdown: null));

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        var post = Assert.Single(Assert.Single(result.Value.Renderings).Posts);
        Assert.Equal("Read ".Length + 23 + punctuation.Length, post.Count);
    }

    public void Dispose() => _host.Dispose();
}
