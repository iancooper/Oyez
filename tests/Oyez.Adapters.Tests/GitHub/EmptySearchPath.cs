namespace Oyez.Adapters.Tests.GitHub;

public sealed class EmptySearchPath : IDisposable
{
    private const string PATH = "PATH";
    private readonly TemporaryFolder _folder = new();
    private readonly string? _original = Environment.GetEnvironmentVariable(PATH);

    public EmptySearchPath() => Environment.SetEnvironmentVariable(PATH, _folder.Path);

    public void Dispose()
    {
        Environment.SetEnvironmentVariable(PATH, _original);
        _folder.Dispose();
    }
}
