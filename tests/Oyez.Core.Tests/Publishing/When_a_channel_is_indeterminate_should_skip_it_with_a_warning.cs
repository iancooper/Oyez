using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class SkipIndeterminateChannelTests : OyezApplicationTest
{
    [Fact]
    public async Task When_a_channel_is_indeterminate_should_skip_it_with_a_warning()
    {
        //Arrange
        Host.Announcements.Add(PublishingTestData.Announcement(
            Id.Value, "x", null, AnnouncementText.OfLength(250, 'a'), AnnouncementText.OfLength(250, 'b'), AnnouncementText.OfLength(250, 'c')));
        var posts = new[]
        {
            new PublishedPost(0, new PostRef("x-1", null), DateTimeOffset.UnixEpoch),
            new PublishedPost(1, new PostRef("x-2", null), DateTimeOffset.UnixEpoch)
        };
        var indeterminate = new ChannelPublication(ChannelState.Indeterminate, "sha256:earlier", posts, "timed out creating post 3");
        var record = new PublicationRecord(new Dictionary<ChannelId, ChannelPublication> { [ChannelId.X] = indeterminate });
        Host.PublicationRecords.Save(Id, record);
        var command = new PublishDueAnnouncements();

        //Act
        await Host.CommandProcessor.SendAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        Assert.Empty(Host.PublisherFor(ChannelId.X).Requests);
        Assert.Equal(indeterminate, Host.PublicationRecords.Load(Id).Channels[ChannelId.X]);
        var warning = Assert.Single(command.Result!.Outcomes);
        Assert.Equal((ChannelId.X, ChannelState.Indeterminate, "timed out creating post 3"), (warning.Channel, warning.State, warning.Reason));
    }
}
