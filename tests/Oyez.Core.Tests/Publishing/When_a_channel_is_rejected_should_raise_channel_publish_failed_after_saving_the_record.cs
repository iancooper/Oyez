using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public sealed class ChannelPublishFailedTests : OyezApplicationTest
{
    private readonly OrderedLog _log;

    public ChannelPublishFailedTests() : this(new OrderedLog())
    {
    }

    private ChannelPublishFailedTests(OrderedLog log) : base(publicationRecords: new RecordingPublicationRecordStore(log), log: log) =>
        _log = log;

    [Fact]
    public async Task When_a_channel_is_rejected_should_raise_channel_publish_failed_after_saving_the_record()
    {
        //Arrange
        Host.Announcements.Add(PublishingTestData.Announcement(Id, "x", ["Released"]));
        Host.PublisherFor(ChannelId.X).RespondFrom(index: 0, result: new PostResult.Rejected("account suspended"));

        //Act
        await Host.CommandProcessor.SendAsync(new PublishDueAnnouncements(), cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal(["record saved", "channel failed: x"], _log.Entries);
        var failure = Assert.Single(SeenChannelFailures.Events);
        Assert.Equal(Id, failure.Announcement);
        Assert.Equal(ChannelId.X, failure.Channel);
        Assert.Equal(ChannelState.Failed, failure.State);
        Assert.Equal("account suspended", failure.Reason);
    }
}
