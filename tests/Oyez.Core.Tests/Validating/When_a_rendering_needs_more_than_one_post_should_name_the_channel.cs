using Oyez.Core.Announcements;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidateOverflowNamesChannelTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_a_rendering_needs_more_than_one_post_should_name_the_channel()
    {
        //Arrange
        var segments = Enumerable.Range(0, 11).Select(_ => AnnouncementText.OfLength(280, 'a')).ToArray();
        _host.Announcements.Add(new AnnouncementDocument(_id, AnnouncementText.With("linkedin", segments), WriteAsMarkdown: null));

        //Act
        var errors = await _host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([_id]), TestContext.Current.CancellationToken);

        //Assert
        var error = Assert.Single(errors);
        Assert.Contains("LinkedIn", error.Message);
    }

    public void Dispose() => _host.Dispose();
}
