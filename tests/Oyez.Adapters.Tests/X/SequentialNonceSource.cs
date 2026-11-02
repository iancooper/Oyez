namespace Oyez.Adapters.Tests.X;

public sealed class SequentialNonceSource : IAmANonceSource
{
    private int _count;

    public string Next() => $"nonce{Interlocked.Increment(ref _count)}";
}
