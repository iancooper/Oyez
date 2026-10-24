using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Preparing;

public class LinkedInHashtagConversionTests : IDisposable
{
    private readonly OyezApplicationHost _host = new(options => options.LinkedIn.ConvertHashtags = false);
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_hashtag_conversion_is_off_should_escape_the_hash_for_linkedin()
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(
            _id,
            """
            title: Release
            channels: LinkedIn
            ---
            Out now for #dotnet
            """,
            WriteAsMarkdown: null));

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        var post = Assert.Single(Assert.Single(result.Value.Renderings).Posts);
        Assert.Equal(@"Out now for \#dotnet", post.Text);
    }

    public void Dispose() => _host.Dispose();
}
