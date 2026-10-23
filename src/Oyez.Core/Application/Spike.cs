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

using Paramore.Brighter;
using Paramore.Darker;

namespace Oyez.Core.Application;

/// <summary>A placeholder command that proves Brighter is wired up.</summary>
/// <param name="text">The text to upper-case.</param>
public sealed class SpikeCommand(string text) : Command(Id.Random())
{
    /// <summary>Gets the text to upper-case.</summary>
    /// <value>The <see cref="string"/> passed to the constructor.</value>
    public string Text { get; } = text;

    /// <summary>Gets or sets the handler's result.</summary>
    /// <value>The upper-cased <see cref="Text"/>, once handled.</value>
    public string? Result { get; set; }
}

/// <summary>Handles <see cref="SpikeCommand"/>.</summary>
public sealed class SpikeCommandHandler : RequestHandlerAsync<SpikeCommand>
{
    /// <summary>Upper-cases the command's text.</summary>
    /// <param name="command">The <see cref="SpikeCommand"/> to handle.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>The handled <see cref="SpikeCommand"/>.</returns>
    public override Task<SpikeCommand> HandleAsync(SpikeCommand command, CancellationToken cancellationToken = default)
    {
        command.Result = command.Text.ToUpperInvariant();
        return base.HandleAsync(command, cancellationToken);
    }
}

/// <summary>A placeholder query that proves Darker is wired up.</summary>
/// <param name="Left">The first number to add.</param>
/// <param name="Right">The second number to add.</param>
public sealed record SpikeQuery(int Left, int Right) : IQuery<int>;

/// <summary>Handles <see cref="SpikeQuery"/>.</summary>
public sealed class SpikeQueryHandler : QueryHandlerAsync<SpikeQuery, int>
{
    /// <summary>Adds the query's numbers.</summary>
    /// <param name="query">The <see cref="SpikeQuery"/> to answer.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>The sum of <see cref="SpikeQuery.Left"/> and <see cref="SpikeQuery.Right"/>.</returns>
    public override Task<int> ExecuteAsync(SpikeQuery query, CancellationToken cancellationToken = default) =>
        Task.FromResult(query.Left + query.Right);
}
