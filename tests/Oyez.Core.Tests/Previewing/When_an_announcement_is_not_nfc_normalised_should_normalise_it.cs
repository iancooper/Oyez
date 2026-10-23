using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Previewing;

public class PreviewUnicodeNormalisationTests : IDisposable
{
    private const string DECOMPOSED = "Café";
    private const string COMPOSED = "Café";

    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_an_announcement_is_not_nfc_normalised_should_normalise_it()
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(
            _id,
            $"title: {DECOMPOSED}\n---\n{DECOMPOSED}",
            WriteAsMarkdown: DECOMPOSED));

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        var preview = result.Value;
        Assert.Equal(COMPOSED, preview.Title);
        Assert.Equal(COMPOSED, preview.Renderings.Single(r => r.Channel == ChannelId.X).Posts[0].Text);
        Assert.Equal(COMPOSED, preview.Renderings.Single(r => r.Channel == ChannelId.WriteAs).Posts[0].Text);
    }

    public void Dispose() => _host.Dispose();
}
