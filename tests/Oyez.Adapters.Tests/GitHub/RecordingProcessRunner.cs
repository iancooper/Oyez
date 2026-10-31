namespace Oyez.Adapters.Tests.GitHub;

public sealed class RecordingProcessRunner(ProcessResult result) : IAmAProcessRunner
{
    public string? FileName { get; private set; }

    public IReadOnlyList<string> Arguments { get; private set; } = [];

    public string? StandardInput { get; private set; }

    public Task<ProcessResult> RunAsync(string fileName, IReadOnlyList<string> arguments, string? standardInput, CancellationToken cancellationToken)
    {
        FileName = fileName;
        Arguments = arguments;
        StandardInput = standardInput;
        return Task.FromResult(result);
    }
}
