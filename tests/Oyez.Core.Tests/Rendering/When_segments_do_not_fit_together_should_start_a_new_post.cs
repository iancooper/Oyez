using Oyez.Core.Announcements;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Rendering;

public class GreedyMergeTests : IDisposable
{
    private readonly OyezApplicationHost _host = new(options => options.Mastodon.Limit = 450);
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_segments_do_not_fit_together_should_start_a_new_post()
    {
        //Arrange
        var first = Segments.OfLength(200, 'a');
        var second = Segments.OfLength(200, 'b');
        var third = Segments.OfLength(200, 'c');
        _host.Announcements.Add(new AnnouncementDocument(_id, Segments.Announcement("mastodon", first, second, third), WriteAsMarkdown: null));

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        var posts = Assert.Single(result.Value.Renderings).Posts;
        Assert.Equal([$"{first}\n\n{second}", third], posts.Select(p => p.Text));
        Assert.Equal([402, 200], posts.Select(p => p.Count));
    }

    public void Dispose() => _host.Dispose();
}
