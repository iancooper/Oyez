using Oyez.Core.Announcements;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Rendering;

public class DeterministicRenderingTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_rendering_twice_should_give_identical_renderings_and_hashes()
    {
        //Arrange
        var segments = new[] { Segments.OfLength(250, 'a'), Segments.OfLength(250, 'b') };
        _host.Announcements.Add(new AnnouncementDocument(_id, Segments.Announcement("x, mastodon", segments), WriteAsMarkdown: null));

        //Act
        var first = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);
        var second = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal(first.Value.Renderings.Select(r => r.Hash), second.Value.Renderings.Select(r => r.Hash));
        Assert.Equal(first.Value.Renderings.Select(r => r.Posts.Select(p => p.Text)), second.Value.Renderings.Select(r => r.Posts.Select(p => p.Text)));
        Assert.All(first.Value.Renderings, r => Assert.StartsWith("sha256:", r.Hash));
        Assert.NotEqual(first.Value.Renderings[0].Hash, first.Value.Renderings[1].Hash);
    }

    public void Dispose() => _host.Dispose();
}
