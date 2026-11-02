namespace Oyez.Adapters.Tests;

public sealed class TemporaryFolder : IDisposable
{
    public TemporaryFolder() => Directory.CreateDirectory(Path);

    public string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "oyez-tests-" + Guid.NewGuid().ToString("N"));

    public void WriteFile(string relativePath, string contents)
    {
        var fullPath = System.IO.Path.Combine(Path, relativePath);
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullPath)!);
        File.WriteAllText(fullPath, contents);
    }

    public void Dispose() => Directory.Delete(Path, recursive: true);
}
