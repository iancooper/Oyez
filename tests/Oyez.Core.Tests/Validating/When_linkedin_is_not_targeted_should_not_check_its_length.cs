using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Validating;

namespace Oyez.Core.Tests.Validating;

public class ValidateLinkedInNotTargetedTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Fact]
    public async Task When_linkedin_is_not_targeted_should_not_check_its_length()
    {
        //Arrange
        var segments = Enumerable.Range(0, 11).Select(_ => AnnouncementText.OfLength(280, 'a')).ToArray();
        _host.Announcements.Add(new AnnouncementDocument(_id, AnnouncementText.With("x,mastodon,writeas", segments), WriteAsMarkdown: null));

        //Act
        var errors = await _host.QueryProcessor.ExecuteAsync(new ValidateAnnouncements([_id]), TestContext.Current.CancellationToken);

        //Assert
        Assert.Empty(errors);
    }

    public void Dispose() => _host.Dispose();
}
