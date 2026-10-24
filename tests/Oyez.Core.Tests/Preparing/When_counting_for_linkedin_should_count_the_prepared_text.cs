using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Preparing;

public class LinkedInPreparedCountTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_counting_for_linkedin_should_count_the_prepared_text()
    {
        //Arrange
        const string RAW_TEXT = "Use *bold* and #dotnet";
        const int ESCAPED_CHARACTERS = 2;
        const int HASHTAG_TEMPLATE_OVERHEAD = 12;
        _host.Announcements.Add(new AnnouncementDocument(
            _id,
            $"""
            title: Release
            channels: LinkedIn
            ---
            {RAW_TEXT}
            """,
            WriteAsMarkdown: null));

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        var post = Assert.Single(Assert.Single(result.Value.Renderings).Posts);
        Assert.Equal(RAW_TEXT.Length + ESCAPED_CHARACTERS + HASHTAG_TEMPLATE_OVERHEAD, post.Count);
    }

    public void Dispose() => _host.Dispose();
}
