using Oyez.Core.Announcements;
using Paramore.Darker;

namespace Oyez.Core.Previewing;

internal sealed class PreviewAnnouncementHandler(IAmAnAnnouncementStore store)
    : QueryHandlerAsync<PreviewAnnouncement, Result<Preview>>
{
    public override Task<Result<Preview>> ExecuteAsync(PreviewAnnouncement query, CancellationToken cancellationToken = default)
    {
        var announcement = AnnouncementParser.Parse(store.Load(query.Id)!).Value;

        var preview = new Preview(
            announcement.Id,
            announcement.Title,
            announcement.PublishAt,
            Renderer.RenderTargetedChannels(announcement));

        return Task.FromResult(Result<Preview>.Success(preview));
    }
}
