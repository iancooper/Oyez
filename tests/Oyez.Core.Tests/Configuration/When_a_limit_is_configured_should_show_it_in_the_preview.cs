using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Configuration;

public class ConfiguredChannelLimitTests() : OyezApplicationTest(options => options.Mastodon.Limit = 500)
{
    [Fact]
    public async Task When_a_limit_is_configured_should_show_it_in_the_preview()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("mastodon", "Segment"), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal(500, Assert.Single(result.Value.Renderings).Limit);
    }
}
