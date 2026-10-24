using Oyez.Core.Announcements;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Rendering;

public class MergeOnTheLimitTests : IDisposable
{
    private readonly OyezApplicationHost _host = new(options => options.Mastodon.Limit = 402);
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_a_merge_lands_exactly_on_the_limit_should_merge()
    {
        //Arrange
        var segments = new[] { Segments.OfLength(200, 'a'), Segments.OfLength(200, 'b') };
        _host.Announcements.Add(new AnnouncementDocument(_id, Segments.Announcement("mastodon", segments), WriteAsMarkdown: null));

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal(402, Assert.Single(Assert.Single(result.Value.Renderings).Posts).Count);
    }

    public void Dispose() => _host.Dispose();
}
