using Oyez.Core.Announcements;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidatePostOverLimitNamesChannelTests() : OyezApplicationTest(options => options.Mastodon.Limit = 100)
{
    [Fact]
    public async Task When_a_post_is_over_its_channels_limit_should_name_the_channel()
    {
        //Arrange
        var segments = new[] { AnnouncementText.OfLength(10, 'a'), AnnouncementText.OfLength(101, 'b') };
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("mastodon", segments), WriteAsMarkdown: null));

        //Act
        var errors = await Host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([Id]), TestContext.Current.CancellationToken);

        //Assert
        var error = Assert.Single(errors);
        Assert.Contains("Mastodon", error.Message);
    }
}
