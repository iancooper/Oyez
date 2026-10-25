using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Oyez.Core.Announcements;
using Oyez.Core.Application;
using Oyez.Core.Channels;
using Oyez.Core.Configuration;
using Oyez.Core.Publishing;
using Paramore.Brighter;
using Paramore.Darker;

namespace Oyez.Core.Tests;

/// <summary>
/// Builds Oyez's application layer the way production does, with in-memory ports in place of I/O.
/// </summary>
/// <remarks>
/// Brighter and Darker capture a logger factory in static state, so every host shares one that is never disposed.
/// </remarks>
public sealed class OyezApplicationHost : IDisposable
{
    private readonly ServiceProvider _provider;
    private readonly Dictionary<ChannelId, InMemoryChannelPublisher> _publishers =
        ChannelId.All.ToDictionary(channel => channel, channel => new InMemoryChannelPublisher(channel));

    public OyezApplicationHost(Action<OyezOptions>? configure = null, IAmAPublicationRecordStore? publicationRecords = null, OrderedLog? log = null, IReadOnlyCollection<ChannelId>? channelsWithoutPublisher = null)
    {
        PublicationRecords = publicationRecords ?? new InMemoryPublicationRecordStore();
        var services = new ServiceCollection()
            .AddLogging()
            .AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance)
            .AddSingleton<TimeProvider>(Clock)
            .AddSingleton<IAmAnAnnouncementStore>(Announcements)
            .AddSingleton(PublicationRecords)
            .AddSingleton(log ?? new OrderedLog());
        foreach (var (channel, publisher) in _publishers.Where(entry => channelsWithoutPublisher?.Contains(entry.Key) != true))
        {
            services.AddSingleton<IAmAChannelPublisher>(publisher);
        }

        services.AddOyezApplication(configure);

        _provider = services.BuildServiceProvider();
    }

    public InMemoryAnnouncementStore Announcements { get; } = new();

    public IAmAPublicationRecordStore PublicationRecords { get; }

    public FakeTimeProvider Clock { get; } = new(new DateTimeOffset(2026, 11, 3, 12, 0, 0, TimeSpan.Zero));

    public IQueryProcessor QueryProcessor => _provider.GetRequiredService<IQueryProcessor>();

    public IAmACommandProcessor CommandProcessor => _provider.GetRequiredService<IAmACommandProcessor>();

    public InMemoryChannelPublisher PublisherFor(ChannelId channel) => _publishers[channel];

    public void Dispose() => _provider.Dispose();
}
