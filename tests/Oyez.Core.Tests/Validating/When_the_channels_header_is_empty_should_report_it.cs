using Oyez.Core.Announcements;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidateEmptyChannelsTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_the_channels_header_is_empty_should_report_it()
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(_id, AnnouncementText.With(string.Empty, "Segment"), WriteAsMarkdown: null));

        //Act
        var errors = await _host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([_id]), TestContext.Current.CancellationToken);

        //Assert
        var error = Assert.Single(errors);
        Assert.Equal(_id, error.Announcement);
        Assert.Contains("channels", error.Message);
        Assert.Null(error.Channel);
    }

    public void Dispose() => _host.Dispose();
}
