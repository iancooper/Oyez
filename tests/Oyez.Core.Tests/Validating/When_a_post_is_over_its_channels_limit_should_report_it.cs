using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidatePostOverChannelLimitTests() : OyezApplicationTest(options => options.Mastodon.Limit = 100)
{
    [Fact]
    public async Task When_a_post_is_over_its_channels_limit_should_report_it()
    {
        //Arrange
        var segments = new[] { AnnouncementText.OfLength(10, 'a'), AnnouncementText.OfLength(101, 'b') };
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("mastodon", segments), WriteAsMarkdown: null));

        //Act
        var errors = await Host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([Id]), TestContext.Current.CancellationToken);

        //Assert
        var error = Assert.Single(errors);
        Assert.Equal(Id, error.Announcement);
        Assert.Equal(ChannelId.Mastodon, error.Channel);
        Assert.Equal(2, error.Segment);
    }
}
