using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Oyez.Adapters.Tests;

public sealed record LogEntry(LogLevel Level, string Message);

public sealed class CollectedLogs : ILoggerProvider
{
    private readonly List<LogEntry> _entries = [];

    public IReadOnlyList<LogEntry> Entries => _entries;

    public void CaptureIn(IServiceCollection services) =>
        services.AddLogging(logging => logging.AddProvider(this));

    public ILogger CreateLogger(string categoryName) => new CollectingLogger(_entries);

    public void Dispose()
    {
    }

    private sealed class CollectingLogger(List<LogEntry> entries) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
            entries.Add(new LogEntry(logLevel, formatter(state, exception)));
    }
}
