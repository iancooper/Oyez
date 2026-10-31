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
using Oyez.Core.Publishing;

namespace Oyez.Adapters;

/// <summary>
/// Registers the platform adapters.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers an <c>IAmAChannelPublisher</c> for each platform.
    /// </summary>
    /// <param name="services">The services to add to.</param>
    /// <param name="configure">Sets the <see cref="AdapterOptions"/>.</param>
    /// <returns>The same <paramref name="services"/>, for chaining.</returns>
    public static IServiceCollection AddOyezAdapters(this IServiceCollection services, Action<AdapterOptions> configure)
    {
        var options = new AdapterOptions();
        configure(options);

        services.AddHttpClient<WriteAsPublisher>().AddTypedClient((http, _) => new WriteAsPublisher(http, options.WriteAs));
        services.AddTransient<IAmAChannelPublisher>(provider => provider.GetRequiredService<WriteAsPublisher>());

        return services;
    }
}
