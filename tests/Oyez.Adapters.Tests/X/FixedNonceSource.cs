namespace Oyez.Adapters.Tests.X;

public sealed class FixedNonceSource(string nonce) : IAmANonceSource
{
    public string Next() => nonce;
}
