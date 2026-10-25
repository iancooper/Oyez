using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Merging;

public class PostCountPerChannelTests : OyezApplicationTest
{
    [Fact]
    public async Task When_rendering_four_long_segments_should_give_each_channel_its_expected_post_count()
    {
        //Arrange
        var segments = new[] { 'a', 'b', 'c', 'd' }.Select(fill => AnnouncementText.OfLength(250, fill)).ToArray();
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("x, mastodon, linkedin, writeas", segments), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        var postCounts = result.Value.Renderings.ToDictionary(r => r.Channel, r => r.Posts.Count);
        Assert.Equal(4, postCounts[ChannelId.X]);
        Assert.Equal(1, postCounts[ChannelId.Mastodon]);
        Assert.Equal(1, postCounts[ChannelId.LinkedIn]);
        Assert.Equal(1, postCounts[ChannelId.WriteAs]);
    }
}
