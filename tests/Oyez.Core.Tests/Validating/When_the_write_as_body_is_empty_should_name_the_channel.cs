using Oyez.Core.Announcements;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidateEmptyWriteAsBodyNamesChannelTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_the_write_as_body_is_empty_should_name_the_channel()
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(_id, AnnouncementText.With("writeas", "One"), WriteAsMarkdown: "   "));

        //Act
        var errors = await _host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([_id]), TestContext.Current.CancellationToken);

        //Assert
        var error = Assert.Single(errors);
        Assert.Contains("Write.as", error.Message);
    }

    public void Dispose() => _host.Dispose();
}
