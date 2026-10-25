using Oyez.Core.Announcements;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidateOverflowNamesChannelTests : OyezApplicationTest
{
    [Fact]
    public async Task When_a_rendering_needs_more_than_one_post_should_name_the_channel()
    {
        //Arrange
        var segments = Enumerable.Range(0, 11).Select(_ => AnnouncementText.OfLength(280, 'a')).ToArray();
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("linkedin", segments), WriteAsMarkdown: null));

        //Act
        var errors = await Host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([Id]), TestContext.Current.CancellationToken);

        //Assert
        var error = Assert.Single(errors);
        Assert.Contains("LinkedIn", error.Message);
    }
}
