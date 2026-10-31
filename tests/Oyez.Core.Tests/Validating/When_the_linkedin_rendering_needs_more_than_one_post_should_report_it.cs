using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidateLinkedInOverflowTests : OyezApplicationTest
{
    [Fact]
    public async Task When_the_linkedin_rendering_needs_more_than_one_post_should_report_it()
    {
        //Arrange
        var segments = Enumerable.Range(0, 11).Select(_ => AnnouncementText.OfLength(280, 'a')).ToArray();
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("linkedin", segments), WriteAsMarkdown: null));

        //Act
        var errors = await Host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([Id]), TestContext.Current.CancellationToken);

        //Assert
        var error = Assert.Single(errors);
        Assert.Equal(Id, error.Announcement);
        Assert.Equal(ChannelId.LinkedIn, error.Channel);
        Assert.Null(error.Segment);
    }
}
