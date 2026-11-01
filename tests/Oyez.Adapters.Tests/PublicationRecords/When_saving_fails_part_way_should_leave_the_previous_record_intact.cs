using System.Runtime.Versioning;
using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.PublicationRecords;

public class FailedSavingRecordTests
{
    [Fact]
    [UnsupportedOSPlatform("windows")]
    public void When_saving_fails_part_way_should_leave_the_previous_record_intact()
    {
        Assert.SkipWhen(OperatingSystem.IsWindows(), "File permissions are not enforced the same way on Windows.");
        Assert.SkipWhen(Environment.UserName == "root", "Root ignores file permissions.");

        //Arrange
        using var folder = new TemporaryFolder();
        var id = new AnnouncementId("2026-11-03-brighter-10-3-0");
        var announcementFolder = Path.Combine(folder.Path, id.Value);
        using var host = new OyezAdaptersHost(options => options.AnnouncementsPath = folder.Path);
        var store = host.Service<IAmAPublicationRecordStore>();
        var previous = PublicationRecordTestData.RecordWithEveryState();
        store.Save(id, previous);
        var replacement = new PublicationRecord(new Dictionary<ChannelId, ChannelPublication>
        {
            [ChannelId.X] = new(ChannelState.Failed, new RenderingHash("sha256:eeee"), [], "X rejected the post")
        });
        File.SetUnixFileMode(announcementFolder, UnixFileMode.UserRead | UnixFileMode.UserExecute);

        Exception? failure;
        try
        {
            //Act
            failure = Record.Exception(() => store.Save(id, replacement));
        }
        finally
        {
            File.SetUnixFileMode(announcementFolder, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
        }

        //Assert
        Assert.NotNull(failure);
        PublicationRecordTestData.AssertSame(previous, store.Load(id));
        Assert.Equal(["published.json"], Directory.GetFiles(announcementFolder).Select(Path.GetFileName));
    }
}
