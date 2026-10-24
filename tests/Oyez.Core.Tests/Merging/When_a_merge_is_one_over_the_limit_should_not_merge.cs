using Oyez.Core.Announcements;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Merging;

public class MergeOverTheLimitTests : IDisposable
{
    private readonly OyezApplicationHost _host = new(options => options.Mastodon.Limit = 401);
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_a_merge_is_one_over_the_limit_should_not_merge()
    {
        //Arrange
        var segments = new[] { AnnouncementText.OfLength(200, 'a'), AnnouncementText.OfLength(200, 'b') };
        _host.Announcements.Add(new AnnouncementDocument(_id, AnnouncementText.With("mastodon", segments), WriteAsMarkdown: null));

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal([200, 200], Assert.Single(result.Value.Renderings).Posts.Select(p => p.Count));
    }

    public void Dispose() => _host.Dispose();
}
