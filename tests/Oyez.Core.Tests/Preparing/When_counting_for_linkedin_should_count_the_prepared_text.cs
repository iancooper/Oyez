using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Preparing;

public class LinkedInPreparedCountTests : OyezApplicationTest
{
    [Fact]
    public async Task When_counting_for_linkedin_should_count_the_prepared_text()
    {
        //Arrange
        const string RAW_TEXT = "Use *bold* and #dotnet";
        const int ESCAPED_CHARACTERS = 2;
        const int HASHTAG_TEMPLATE_OVERHEAD = 12;
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("LinkedIn", RAW_TEXT), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        var post = Assert.Single(Assert.Single(result.Value.Renderings).Posts);
        Assert.Equal(RAW_TEXT.Length + ESCAPED_CHARACTERS + HASHTAG_TEMPLATE_OVERHEAD, post.Count);
    }
}
