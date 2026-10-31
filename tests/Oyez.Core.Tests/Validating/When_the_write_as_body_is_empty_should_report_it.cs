using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidateEmptyWriteAsBodyTests : OyezApplicationTest
{
    [Fact]
    public async Task When_the_write_as_body_is_empty_should_report_it()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("writeas", "One"), WriteAsMarkdown: "   "));

        //Act
        var errors = await Host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([Id]), TestContext.Current.CancellationToken);

        //Assert
        var error = Assert.Single(errors);
        Assert.Equal(Id, error.Announcement);
        Assert.Equal(ChannelId.WriteAs, error.Channel);
    }
}
