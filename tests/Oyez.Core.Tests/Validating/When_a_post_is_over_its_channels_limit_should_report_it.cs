using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidatePostOverChannelLimitTests : IDisposable
{
    private readonly OyezApplicationHost _host = new(options => options.Mastodon.Limit = 100);
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_a_post_is_over_its_channels_limit_should_report_it()
    {
        //Arrange
        var segments = new[] { AnnouncementText.OfLength(10, 'a'), AnnouncementText.OfLength(101, 'b') };
        _host.Announcements.Add(new AnnouncementDocument(_id, AnnouncementText.With("mastodon", segments), WriteAsMarkdown: null));

        //Act
        var errors = await _host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([_id]), TestContext.Current.CancellationToken);

        //Assert
        var error = Assert.Single(errors);
        Assert.Equal(_id, error.Announcement);
        Assert.Equal(ChannelId.Mastodon, error.Channel);
        Assert.Equal(2, error.Segment);
    }

    public void Dispose() => _host.Dispose();
}
