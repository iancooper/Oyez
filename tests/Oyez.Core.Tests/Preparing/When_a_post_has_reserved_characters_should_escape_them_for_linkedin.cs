using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Preparing;

public class LinkedInEscapingTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Theory]
    [InlineData('\\')]
    [InlineData('|')]
    [InlineData('{')]
    [InlineData('}')]
    [InlineData('@')]
    [InlineData('[')]
    [InlineData(']')]
    [InlineData('(')]
    [InlineData(')')]
    [InlineData('<')]
    [InlineData('>')]
    [InlineData('#')]
    [InlineData('*')]
    [InlineData('_')]
    [InlineData('~')]
    public async Task When_a_post_has_reserved_characters_should_escape_them_for_linkedin(char reserved)
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(
            _id,
            $"""
            title: Release
            channels: LinkedIn
            ---
            Fast {reserved} release
            """,
            WriteAsMarkdown: null));

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        var post = Assert.Single(Assert.Single(result.Value.Renderings).Posts);
        Assert.Equal($"Fast \\{reserved} release", post.Text);
    }

    public void Dispose() => _host.Dispose();
}
