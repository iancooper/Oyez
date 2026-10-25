using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public sealed class SaveAfterEachPostTests : OyezApplicationTest
{
    private readonly RecordingPublicationRecordStore _records;

    public SaveAfterEachPostTests() : this(new RecordingPublicationRecordStore())
    {
    }

    private SaveAfterEachPostTests(RecordingPublicationRecordStore records) : base(publicationRecords: records) =>
        _records = records;

    [Fact]
    public async Task When_publishing_a_thread_should_save_the_record_after_each_post()
    {
        //Arrange
        Host.Announcements.Add(PublishingTestData.Announcement(Id.Value, "x", null, AnnouncementText.OfLength(250, 'a'), AnnouncementText.OfLength(250, 'b')));

        //Act
        await Host.CommandProcessor.SendAsync(new PublishDueAnnouncements(), cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        var channelAfterEachSave = _records.Saves.Select(record => record.Channels[ChannelId.X]).ToList();
        Assert.Equal([ChannelState.Partial, ChannelState.Complete], channelAfterEachSave.Select(channel => channel.State));
        Assert.Equal([1, 2], channelAfterEachSave.Select(channel => channel.Posts.Count));
    }
}
