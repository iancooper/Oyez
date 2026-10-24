using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Counting;

public class DottedNameCountingForXTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Theory]
    [InlineData("Paramore.Brighter")]
    [InlineData("v10.2.0")]
    [InlineData("e.g.")]
    [InlineData("file.cs")]
    public async Task When_a_post_contains_dotted_names_that_are_not_domains_should_count_them_literally_for_x(string name)
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(
            _id,
            $"""
            title: Release
            channels: X
            ---
            See {name} now
            """,
            WriteAsMarkdown: null));

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        var post = Assert.Single(Assert.Single(result.Value.Renderings).Posts);
        Assert.Equal("See ".Length + name.Length + " now".Length, post.Count);
    }

    public void Dispose() => _host.Dispose();
}
