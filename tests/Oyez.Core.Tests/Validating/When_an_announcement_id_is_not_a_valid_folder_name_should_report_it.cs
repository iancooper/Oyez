using Oyez.Core.Announcements;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidateAnnouncementIdTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("Brighter_10.3.0");

    [Fact]
    public async Task When_an_announcement_id_is_not_a_valid_folder_name_should_report_it()
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(
            _id,
            """
            title: Release
            ---
            Segment
            """,
            WriteAsMarkdown: null));

        //Act
        var errors = await _host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([_id]), TestContext.Current.CancellationToken);

        //Assert
        var error = Assert.Single(errors);
        Assert.Equal(_id, error.Announcement);
        Assert.Contains("Brighter_10.3.0", error.Message);
    }

    public void Dispose() => _host.Dispose();
}
