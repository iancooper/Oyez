using Oyez.Core.Publishing;
using Paramore.Brighter;

namespace Oyez.Core.Tests;

public sealed class RecordingChannelPublishFailedHandler(OrderedLog log, SeenChannelFailures seen) : RequestHandlerAsync<ChannelPublishFailed>
{
    public override Task<ChannelPublishFailed> HandleAsync(ChannelPublishFailed @event, CancellationToken cancellationToken = default)
    {
        log.Record($"channel failed: {@event.Channel}");
        seen.Add(@event);
        return base.HandleAsync(@event, cancellationToken);
    }
}
