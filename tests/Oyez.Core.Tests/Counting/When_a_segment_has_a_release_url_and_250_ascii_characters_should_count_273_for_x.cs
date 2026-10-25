using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Counting;

public class XUrlWeightedCountingTests : OyezApplicationTest
{
    [Fact]
    public async Task When_a_segment_has_a_release_url_and_250_ascii_characters_should_count_273_for_x()
    {
        //Arrange
        const string URL = "https://github.com/BrighterCommand/Brighter/releases/tag/10.2.0";
        var text = $"{URL} {new string('a', 249)}";
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("X", text), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        var post = Assert.Single(Assert.Single(result.Value.Renderings).Posts);
        Assert.Equal(273, post.Count);
    }
}
