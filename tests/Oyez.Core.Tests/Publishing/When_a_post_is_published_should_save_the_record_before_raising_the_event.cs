using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public sealed class SaveBeforeEventTests : OyezApplicationTest
{
    private readonly OrderedLog _log;

    public SaveBeforeEventTests() : this(new OrderedLog())
    {
    }

    private SaveBeforeEventTests(OrderedLog log) : base(publicationRecords: new RecordingPublicationRecordStore(log), log: log) =>
        _log = log;

    [Fact]
    public async Task When_a_post_is_published_should_save_the_record_before_raising_the_event()
    {
        //Arrange
        Host.Announcements.Add(PublishingTestData.Announcement(Id, "x", ["Released"]));

        //Act
        await Host.CommandProcessor.SendAsync(new PublishDueAnnouncements(), cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal(["record saved", "event raised"], _log.Entries);
    }
}
