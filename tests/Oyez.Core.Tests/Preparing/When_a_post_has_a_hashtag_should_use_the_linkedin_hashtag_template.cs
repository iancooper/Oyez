using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Preparing;

public class LinkedInHashtagTests : OyezApplicationTest
{
    [Fact]
    public async Task When_a_post_has_a_hashtag_should_use_the_linkedin_hashtag_template()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("LinkedIn", "Out now for #dotnet and #open_source (see C# notes)"), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        var post = Assert.Single(Assert.Single(result.Value.Renderings).Posts);
        Assert.Equal(@"Out now for {hashtag|\#|dotnet} and {hashtag|\#|open_source} \(see C\# notes\)", post.Text);
    }
}
