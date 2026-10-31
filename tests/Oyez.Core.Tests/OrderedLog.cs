namespace Oyez.Core.Tests;

public sealed class OrderedLog
{
    private readonly List<string> _entries = [];

    public IReadOnlyList<string> Entries => _entries;

    public void Record(string entry) => _entries.Add(entry);
}
