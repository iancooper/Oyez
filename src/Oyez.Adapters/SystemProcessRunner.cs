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

using System.Diagnostics;

namespace Oyez.Adapters;

internal sealed class SystemProcessRunner : IAmAProcessRunner
{
    public async Task<ProcessResult> RunAsync(string fileName, IReadOnlyList<string> arguments, string? standardInput, CancellationToken cancellationToken)
    {
        using var process = Start(fileName, arguments);
        var output = ReadAsync(process.StandardOutput, cancellationToken);
        var errors = ReadAsync(process.StandardError, cancellationToken);
        await process.StandardInput.WriteAsync(standardInput.AsMemory(), cancellationToken);
        process.StandardInput.Close();
        await process.WaitForExitAsync(cancellationToken);
        return new ProcessResult(process.ExitCode, await output + await errors);
    }

    private static Process Start(string fileName, IReadOnlyList<string> arguments)
    {
        var info = new ProcessStartInfo(fileName)
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        foreach (var argument in arguments)
        {
            info.ArgumentList.Add(argument);
        }

        return Process.Start(info) ?? throw new InvalidOperationException($"Could not start {fileName}");
    }

    private static Task<string> ReadAsync(StreamReader reader, CancellationToken cancellationToken) =>
        reader.ReadToEndAsync(cancellationToken);
}
