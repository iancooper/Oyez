using Oyez.Core.Publishing;
using Paramore.Brighter;

namespace Oyez.Core.Tests;

public sealed class RecordingPostPublishedHandler(OrderedLog log) : RequestHandlerAsync<PostPublished>
{
    public override Task<PostPublished> HandleAsync(PostPublished @event, CancellationToken cancellationToken = default)
    {
        log.Record("event raised");
        return base.HandleAsync(@event, cancellationToken);
    }
}
