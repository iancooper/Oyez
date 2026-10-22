using Microsoft.Extensions.DependencyInjection;
using Paramore.Brighter.Extensions.DependencyInjection;
using Paramore.Darker.AspNetCore;

namespace Oyez.Core.Application;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Oyez's application layer: Brighter's command processor and Darker's query processor,
    /// with every command, event and query handler found in Oyez.Core. All requests are handled in-process.
    /// </summary>
    public static IServiceCollection AddOyezApplication(this IServiceCollection services)
    {
        var core = typeof(ServiceCollectionExtensions).Assembly;

        services.AddBrighter().AutoFromAssemblies([core]);
        services.AddDarker().AddHandlersFromAssemblies(core);

        return services;
    }
}
