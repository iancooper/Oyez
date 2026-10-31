using Oyez.Core.Announcements;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidateNoEnabledTargetTests() : OyezApplicationTest(options => options.LinkedIn.Enabled = false)
{
    [Fact]
    public async Task When_every_targeted_channel_is_disabled_should_report_it()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("linkedin", "Segment"), WriteAsMarkdown: null));

        //Act
        var errors = await Host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([Id]), TestContext.Current.CancellationToken);

        //Assert
        var error = Assert.Single(errors);
        Assert.Equal(Id, error.Announcement);
        Assert.Contains("enabled", error.Message);
        Assert.Null(error.Channel);
    }
}
