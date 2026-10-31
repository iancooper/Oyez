using Oyez.Core.Announcements;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidateEmptyChannelsTests : OyezApplicationTest
{
    [Fact]
    public async Task When_the_channels_header_is_empty_should_report_it()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With(string.Empty, "Segment"), WriteAsMarkdown: null));

        //Act
        var errors = await Host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([Id]), TestContext.Current.CancellationToken);

        //Assert
        var error = Assert.Single(errors);
        Assert.Equal(Id, error.Announcement);
        Assert.Contains("channels", error.Message);
        Assert.Null(error.Channel);
    }
}
