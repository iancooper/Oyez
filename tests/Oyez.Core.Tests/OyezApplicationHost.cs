using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Oyez.Core.Announcements;
using Oyez.Core.Application;
using Oyez.Core.Configuration;
using Oyez.Core.Publishing;
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

    public OyezApplicationHost(Action<OyezOptions>? configure = null)
    {
        _provider = new ServiceCollection()
            .AddLogging()
            .AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance)
            .AddSingleton<IAmAnAnnouncementStore>(Announcements)
            .AddSingleton<IAmAPublicationRecordStore>(PublicationRecords)
            .AddOyezApplication(configure)
            .BuildServiceProvider();
    }

    public InMemoryAnnouncementStore Announcements { get; } = new();

    public InMemoryPublicationRecordStore PublicationRecords { get; } = new();

    public IQueryProcessor QueryProcessor => _provider.GetRequiredService<IQueryProcessor>();

    public void Dispose() => _provider.Dispose();
}
