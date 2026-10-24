using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Tests.Rendering;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidateSegmentWhenXNotTargetedTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_x_is_not_targeted_should_still_check_segments_against_xs_limit()
    {
        //Arrange
        var segments = new[] { Segments.OfLength(10, 'a'), Segments.OfLength(281, 'b') };
        _host.Announcements.Add(new AnnouncementDocument(_id, Segments.Announcement("mastodon", segments), WriteAsMarkdown: null));
        //Act
        var errors = await _host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([_id]), TestContext.Current.CancellationToken);

        //Assert
        var error = Assert.Single(errors);
        Assert.Equal(_id, error.Announcement);
        Assert.Equal(2, error.Segment);
        Assert.Equal(ChannelId.X, error.Channel);
    }

    public void Dispose() => _host.Dispose();
}
