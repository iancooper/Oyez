using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidateLinkedInNotTargetedTests : OyezApplicationTest
{
    [Fact]
    public async Task When_linkedin_is_not_targeted_should_not_check_its_length()
    {
        //Arrange
        var segments = Enumerable.Range(0, 11).Select(_ => AnnouncementText.OfLength(280, 'a')).ToArray();
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("x,mastodon,writeas", segments), WriteAsMarkdown: null));

        //Act
        var errors = await Host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([Id]), TestContext.Current.CancellationToken);

        //Assert
        Assert.Empty(errors);
    }
}
