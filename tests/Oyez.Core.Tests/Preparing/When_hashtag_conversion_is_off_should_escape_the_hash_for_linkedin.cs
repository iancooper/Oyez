using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Preparing;

public class LinkedInHashtagConversionTests() : OyezApplicationTest(options => options.LinkedIn.ConvertHashtags = false)
{
    [Fact]
    public async Task When_hashtag_conversion_is_off_should_escape_the_hash_for_linkedin()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("LinkedIn", "Out now for #dotnet"), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        var post = Assert.Single(Assert.Single(result.Value.Renderings).Posts);
        Assert.Equal(@"Out now for \#dotnet", post.Text);
    }
}
