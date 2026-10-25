using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidateSegmentWhenXNotTargetedTests : OyezApplicationTest
{
    [Fact]
    public async Task When_x_is_not_targeted_should_still_check_segments_against_xs_limit()
    {
        //Arrange
        var segments = new[] { AnnouncementText.OfLength(10, 'a'), AnnouncementText.OfLength(281, 'b') };
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("mastodon", segments), WriteAsMarkdown: null));

        //Act
        var errors = await Host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([Id]), TestContext.Current.CancellationToken);

        //Assert
        var error = Assert.Single(errors);
        Assert.Equal(Id, error.Announcement);
        Assert.Equal(2, error.Segment);
        Assert.Equal(ChannelId.X, error.Channel);
    }
}
