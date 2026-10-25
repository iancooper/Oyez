using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Previewing;

public class PreviewUnicodeNormalisationTests : OyezApplicationTest
{
    private const string COMBINING_ACUTE_ACCENT = "\u0301";
    private const string E_WITH_ACUTE_ACCENT = "\u00e9";
    private const string DECOMPOSED = "Cafe" + COMBINING_ACUTE_ACCENT;
    private const string COMPOSED = "Caf" + E_WITH_ACUTE_ACCENT;

    [Fact]
    public async Task When_an_announcement_is_not_nfc_normalised_should_normalise_it()
    {
        //Arrange
        Host.Announcements.Add(new AnnouncementDocument(
            Id,
            $"title: {DECOMPOSED}\n---\n{DECOMPOSED}",
            WriteAsMarkdown: DECOMPOSED));

        //Act
        var result = await Host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(Id), TestContext.Current.CancellationToken);

        //Assert
        var preview = result.Value;
        Assert.Equal(COMPOSED, preview.Title);
        Assert.Equal(COMPOSED, preview.Renderings.Single(r => r.Channel == ChannelId.X).Posts[0].Text);
        Assert.Equal(COMPOSED, preview.Renderings.Single(r => r.Channel == ChannelId.WriteAs).Posts[0].Text);
    }
}
