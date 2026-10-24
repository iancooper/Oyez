using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Rendering;

public class PostCountPerChannelTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_rendering_four_long_segments_should_give_each_channel_its_expected_post_count()
    {
        //Arrange
        var segments = new[] { 'a', 'b', 'c', 'd' }.Select(fill => Segments.OfLength(250, fill)).ToArray();
        _host.Announcements.Add(new AnnouncementDocument(_id, Segments.Announcement("x, mastodon, linkedin, writeas", segments), WriteAsMarkdown: null));

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        var postCounts = result.Value.Renderings.ToDictionary(r => r.Channel, r => r.Posts.Count);
        Assert.Equal(4, postCounts[ChannelId.X]);
        Assert.Equal(1, postCounts[ChannelId.Mastodon]);
        Assert.Equal(1, postCounts[ChannelId.LinkedIn]);
        Assert.Equal(1, postCounts[ChannelId.WriteAs]);
    }

    public void Dispose() => _host.Dispose();
}
