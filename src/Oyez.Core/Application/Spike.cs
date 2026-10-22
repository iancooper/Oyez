using Paramore.Brighter;
using Paramore.Darker;

namespace Oyez.Core.Application;

// T-0 spike: proves Brighter and Darker are wired up. Removed in T-17.

public sealed class SpikeCommand(string text) : Command(Id.Random())
{
    public string Text { get; } = text;
    public string? Result { get; set; }
}

public sealed class SpikeCommandHandler : RequestHandlerAsync<SpikeCommand>
{
    public override Task<SpikeCommand> HandleAsync(SpikeCommand command, CancellationToken cancellationToken = default)
    {
        command.Result = command.Text.ToUpperInvariant();
        return base.HandleAsync(command, cancellationToken);
    }
}

public sealed record SpikeQuery(int Left, int Right) : IQuery<int>;

public sealed class SpikeQueryHandler : QueryHandlerAsync<SpikeQuery, int>
{
    public override Task<int> ExecuteAsync(SpikeQuery query, CancellationToken cancellationToken = default) =>
        Task.FromResult(query.Left + query.Right);
}
