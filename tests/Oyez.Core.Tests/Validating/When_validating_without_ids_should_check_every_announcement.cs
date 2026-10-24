using Oyez.Core.Announcements;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidateEveryAnnouncementTests : IDisposable
{
    private const string UNTITLED = "---\nSegment";
    private const string TITLED = "title: Release\n---\nSegment";

    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _firstUntitled = new("2026-11-03-brighter-10-3-0");
    private readonly AnnouncementId _secondUntitled = new("2026-11-10-darker-3-1-0");
    private readonly AnnouncementId _titled = new("2026-11-17-brighter-10-3-1");

    [Fact]
    public async Task When_validating_without_ids_should_check_every_announcement()
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(_firstUntitled, UNTITLED, WriteAsMarkdown: null));
        _host.Announcements.Add(new AnnouncementDocument(_secondUntitled, UNTITLED, WriteAsMarkdown: null));
        _host.Announcements.Add(new AnnouncementDocument(_titled, TITLED, WriteAsMarkdown: null));

        //Act
        var errors = await _host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements(), TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal([_firstUntitled, _secondUntitled], errors.Select(error => error.Announcement).OrderBy(id => id.Value, StringComparer.Ordinal));
    }

    public void Dispose() => _host.Dispose();
}
