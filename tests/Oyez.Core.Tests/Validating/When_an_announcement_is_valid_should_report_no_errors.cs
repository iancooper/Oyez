using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Tests.Rendering;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidateValidAnnouncementTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_an_announcement_is_valid_should_report_no_errors()
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(_id, Segments.Announcement("x,mastodon,linkedin,writeas", "One", "Two"), WriteAsMarkdown: null));
        //Act
        var errors = await _host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([_id]), TestContext.Current.CancellationToken);

        //Assert
        Assert.Empty(errors);
    }

    public void Dispose() => _host.Dispose();
}
