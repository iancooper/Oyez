using Microsoft.Extensions.DependencyInjection;
using Oyez.Core.Application;
using Paramore.Brighter;
using Paramore.Darker;

namespace Oyez.Core.Tests.Application;

public class CompositionSpikeTests
{
    private static ServiceProvider BuildProvider() =>
        new ServiceCollection()
            .AddLogging()
            .AddOyezApplication()
            .BuildServiceProvider();

    [Fact]
    public async Task A_command_sent_through_Brighter_reaches_its_handler()
    {
        using var provider = BuildProvider();
        var commandProcessor = provider.GetRequiredService<IAmACommandProcessor>();
        var command = new SpikeCommand("oyez");

        await commandProcessor.SendAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal("OYEZ", command.Result);
    }

    [Fact]
    public async Task A_query_executed_through_Darker_returns_its_result()
    {
        using var provider = BuildProvider();
        var queryProcessor = provider.GetRequiredService<IQueryProcessor>();

        var result = await queryProcessor.ExecuteAsync(new SpikeQuery(2, 3), TestContext.Current.CancellationToken);

        Assert.Equal(5, result);
    }
}
