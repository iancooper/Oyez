using Oyez.Core.Announcements;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Merging;

public class DeterministicRenderingTests : OyezApplicationTest
{
    [Fact]
    public async Task When_rendering_twice_should_give_identical_renderings_and_hashes()
    {
        //Arrange
        var segments = new[] { AnnouncementText.OfLength(250, 'a'), AnnouncementText.OfLength(250, 'b') };
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("x, mastodon", segments), WriteAsMarkdown: null));

        //Act
        var first = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);
        var second = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal(first.Value.Renderings.Select(r => r.Hash), second.Value.Renderings.Select(r => r.Hash));
        Assert.Equal(first.Value.Renderings.Select(r => r.Posts.Select(p => p.Text)), second.Value.Renderings.Select(r => r.Posts.Select(p => p.Text)));
        Assert.All(first.Value.Renderings, r => Assert.StartsWith("sha256:", r.Hash));
        Assert.NotEqual(first.Value.Renderings[0].Hash, first.Value.Renderings[1].Hash);
    }
}
