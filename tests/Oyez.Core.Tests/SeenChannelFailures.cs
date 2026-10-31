using Oyez.Core.Publishing;

namespace Oyez.Core.Tests;

public sealed class SeenChannelFailures
{
    private readonly List<ChannelPublishFailed> _events = [];

    public IReadOnlyList<ChannelPublishFailed> Events => _events;

    public void Add(ChannelPublishFailed @event) => _events.Add(@event);
}
