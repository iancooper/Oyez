using Oyez.Core.Announcements;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidateEmptyWriteAsBodyNamesChannelTests : OyezApplicationTest
{
    [Fact]
    public async Task When_the_write_as_body_is_empty_should_name_the_channel()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("writeas", "One"), WriteAsMarkdown: "   "));

        //Act
        var errors = await Host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([Id]), TestContext.Current.CancellationToken);

        //Assert
        var error = Assert.Single(errors);
        Assert.Contains("Write.as", error.Message);
    }
}
