#region Licence

/* The MIT License (MIT)
Copyright © 2026 Ian Cooper

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE. */

#endregion

using Microsoft.Extensions.DependencyInjection;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests;

public sealed class OyezAdaptersHost : IDisposable
{
    private readonly ServiceProvider _provider;

    public OyezAdaptersHost(Action<AdapterOptions> configure)
    {
        _provider = new ServiceCollection()
            .AddLogging()
            .ConfigureHttpClientDefaults(client => client.ConfigurePrimaryHttpMessageHandler(() => Http))
            .AddOyezAdapters(configure)
            .BuildServiceProvider();
    }

    public FakeHttpMessageHandler Http { get; } = new();

    public IAmAChannelPublisher PublisherFor(ChannelId channel) =>
        _provider.GetServices<IAmAChannelPublisher>().Single(publisher => publisher.Channel == channel);

    public void Dispose() => _provider.Dispose();
}
