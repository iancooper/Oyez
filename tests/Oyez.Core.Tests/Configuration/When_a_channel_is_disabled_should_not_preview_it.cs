using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Configuration;

public class DisabledChannelTests() : OyezApplicationTest(options => options.LinkedIn.Enabled = false)
{
    [Fact]
    public async Task When_a_channel_is_disabled_should_not_preview_it()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(Id, AnnouncementText.With("x, linkedin", "Segment"), WriteAsMarkdown: null));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal([ChannelId.X], result.Value.Renderings.Select(r => r.Channel));
    }
}
