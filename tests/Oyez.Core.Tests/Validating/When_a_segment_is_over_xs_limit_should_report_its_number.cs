using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidateSegmentOverXLimitTests : OyezApplicationTest
{
    [Fact]
    public async Task When_a_segment_is_over_xs_limit_should_report_its_number()
    {
        //Arrange
        var segments = new[] { AnnouncementText.OfLength(10, 'a'), AnnouncementText.OfLength(281, 'b') };
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("x", segments), WriteAsMarkdown: null));

        //Act
        var errors = await Host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([Id]), TestContext.Current.CancellationToken);

        //Assert
        var error = Assert.Single(errors);
        Assert.Equal(Id, error.Announcement);
        Assert.Equal(2, error.Segment);
        Assert.Equal(ChannelId.X, error.Channel);
    }
}
