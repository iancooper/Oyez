using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Counting;

public class BareDomainCountingForMastodonTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Theory]
    [InlineData("brighter.io")]
    [InlineData("asp.net")]
    [InlineData("www.example.com/path")]
    public async Task When_a_post_contains_a_bare_domain_should_count_it_literally_for_mastodon(string domain)
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(
            _id,
            $"""
            title: Release
            channels: Mastodon
            ---
            Visit {domain} now
            """,
            WriteAsMarkdown: null));

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        var post = Assert.Single(Assert.Single(result.Value.Renderings).Posts);
        Assert.Equal("Visit ".Length + domain.Length + " now".Length, post.Count);
    }

    public void Dispose() => _host.Dispose();
}
