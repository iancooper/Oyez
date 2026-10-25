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

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Oyez.Core.Channels;
using Oyez.Core.Configuration;
using Oyez.Core.Costing;
using Oyez.Core.Publishing;
using Oyez.Core.Validating;
using Paramore.Brighter;
using Paramore.Brighter.Extensions.DependencyInjection;
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
    /// <param name="configure">Optionally changes the default <see cref="OyezOptions"/>.</param>
    /// <returns>The same <see cref="IServiceCollection"/>, for chaining.</returns>
    public static IServiceCollection AddOyezApplication(this IServiceCollection services, Action<OyezOptions>? configure = null)
    {
        var options = new OyezOptions();
        configure?.Invoke(options);
        services.AddSingleton(options);

        var core = typeof(ServiceCollectionExtensions).Assembly;

        services.AddBrighter().AutoFromAssemblies([core]).AsyncHandlers(registry => RegisterInternalCommandHandlers(registry, core));
        services.TryAddSingleton(TimeProvider.System);
        services.AddDarker().AddHandlers(registry => RegisterQueryHandlers(registry, core));

        services.AddSingleton<IAmAChannelProfile, XProfile>();
        services.AddSingleton<IAmAChannelProfile, MastodonProfile>();
        services.AddSingleton<IAmAChannelProfile, LinkedInProfile>();
        services.AddSingleton<IAmAChannelProfile, WriteAsProfile>();
        services.AddSingleton<Renderer>();
        services.AddSingleton<Validator>();
        services.AddSingleton<XCostEstimator>();
        services.AddSingleton<PublishService>();

        return services;
    }

    private static readonly MethodInfo REGISTER_ASYNC = typeof(IAmAnAsyncSubcriberRegistry).GetMethods()
        .Single(method => method.Name == nameof(IAmAnAsyncSubcriberRegistry.RegisterAsync) && method.GetGenericArguments().Length == 2);

    // Brighter's assembly scan only finds public handlers, and most of Oyez's are internal.
    private static void RegisterInternalCommandHandlers(IAmAnAsyncSubcriberRegistry registry, Assembly assembly)
    {
        foreach (var (request, handler) in InternalCommandHandlersIn(assembly))
        {
            REGISTER_ASYNC.MakeGenericMethod(request, handler).Invoke(registry, null);
        }
    }

    private static IEnumerable<(Type Request, Type Handler)> InternalCommandHandlersIn(Assembly assembly) =>
        from type in assembly.GetTypes()
        where type is { IsClass: true, IsAbstract: false, IsVisible: false }
        where type.BaseType is { IsGenericType: true } baseType && baseType.GetGenericTypeDefinition() == typeof(RequestHandlerAsync<>)
        select (type.BaseType!.GetGenericArguments()[0], type);

    // Darker's own assembly scan only finds public handlers, and most of Oyez's are internal.
    private static void RegisterQueryHandlers(IQueryHandlerRegistry registry, Assembly assembly)
    {
        foreach (var (query, result, handler) in QueryHandlersIn(assembly))
        {
            registry.Register(query, result, handler);
        }
    }

    private static IEnumerable<(Type Query, Type Result, Type Handler)> QueryHandlersIn(Assembly assembly) =>
        from type in assembly.GetTypes()
        where type is { IsClass: true, IsAbstract: false }
        from contract in type.GetInterfaces()
        where contract.IsGenericType && contract.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)
        let arguments = contract.GetGenericArguments()
        select (arguments[0], arguments[1], type);
}
