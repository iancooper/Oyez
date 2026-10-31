using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidateValidAnnouncementTests : OyezApplicationTest
{
    [Fact]
    public async Task When_an_announcement_is_valid_should_report_no_errors()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("x,mastodon,linkedin,writeas", "One", "Two"), WriteAsMarkdown: null));

        //Act
        var errors = await Host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([Id]), TestContext.Current.CancellationToken);

        //Assert
        Assert.Empty(errors);
    }
}
