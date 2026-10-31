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

namespace Oyez.Adapters;

/// <summary>
/// Runs an external program and waits for it to finish.
/// </summary>
/// <remarks>
/// Register an implementation before <c>AddOyezAdapters</c> to replace the default, for example to keep a test from running <c>gh</c>.
/// </remarks>
public interface IAmAProcessRunner
{
    /// <summary>Runs a program.</summary>
    /// <param name="fileName">The program to run.</param>
    /// <param name="arguments">The arguments, passed one by one with no shell in between.</param>
    /// <param name="standardInput">Text to write to the program's standard input, or <see langword="null"/> to write none.</param>
    /// <param name="cancellationToken">Cancels the run.</param>
    /// <returns>The <see cref="ProcessResult"/> of the run.</returns>
    Task<ProcessResult> RunAsync(string fileName, IReadOnlyList<string> arguments, string? standardInput, CancellationToken cancellationToken);
}
