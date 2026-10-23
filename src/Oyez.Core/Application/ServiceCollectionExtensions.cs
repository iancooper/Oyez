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
using Paramore.Brighter.Extensions.DependencyInjection;
using Oyez.Core.Previewing;
using Paramore.Darker;
using Paramore.Darker.AspNetCore;

namespace Oyez.Core.Application;

/// <summary>
/// Registers Oyez's application layer with a <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Brighter's command processor and Darker's query processor, with every command, event
    /// and query handler in Oyez.Core. All requests are handled in-process.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to register with.</param>
    /// <returns>The same <see cref="IServiceCollection"/>, for chaining.</returns>
    public static IServiceCollection AddOyezApplication(this IServiceCollection services)
    {
        var core = typeof(ServiceCollectionExtensions).Assembly;

        services.AddBrighter().AutoFromAssemblies([core]);
        services.AddDarker()
            .AddHandlersFromAssemblies(core)
            .AddHandlers(RegisterQueryHandlers);

        return services;
    }

    // Darker's assembly scan only finds public handlers; Oyez's handlers are internal, so they are registered here.
    private static void RegisterQueryHandlers(IQueryHandlerRegistry registry)
    {
        registry.Register<PreviewAnnouncement, Result<Preview>, PreviewAnnouncementHandler>();
    }
}
