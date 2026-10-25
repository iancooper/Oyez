using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class IdempotencyKeyTests : OyezApplicationTest
{
    [Fact]
    public async Task When_publishing_should_give_each_post_an_idempotency_key()
    {
        //Arrange
        Host.Announcements.Add(PublishingTestData.Announcement(Id, "x", [AnnouncementText.OfLength(250, 'a'), AnnouncementText.OfLength(250, 'b')]));

        //Act
        await Host.CommandProcessor.SendAsync(new PublishDueAnnouncements(), cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        var requests = Host.PublisherFor(ChannelId.X).Requests;
        var fingerprint = requests[0].Rendering.Hash.Value["sha256:".Length..][..12];
        Assert.Equal($"oyez:{Id}:x:0:{fingerprint}", requests[0].IdempotencyKey);
        Assert.Equal($"oyez:{Id}:x:1:{fingerprint}", requests[1].IdempotencyKey);
    }
}
