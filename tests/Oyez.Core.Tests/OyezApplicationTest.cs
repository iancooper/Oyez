using Oyez.Core.Announcements;
using Oyez.Core.Configuration;

namespace Oyez.Core.Tests;

/// <summary>
/// Gives a test class an <see cref="OyezApplicationHost"/> and the ID of the announcement most tests use.
/// </summary>
/// <param name="configure">Changes the options the host is built with, when the test needs non-default limits or channels.</param>
public abstract class OyezApplicationTest(Action<OyezOptions>? configure = null) : IDisposable
{
    protected OyezApplicationHost Host { get; } = new(configure);

    protected AnnouncementId Id { get; } = new("2026-11-03-brighter-10-3-0");

    public void Dispose() => Host.Dispose();
}
