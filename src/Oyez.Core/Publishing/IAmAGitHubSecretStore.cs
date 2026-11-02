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

namespace Oyez.Core.Publishing;

/// <summary>
/// Keeps the secrets and variables of the GitHub repository that runs the publishing workflow.
/// </summary>
public interface IAmAGitHubSecretStore
{
    /// <summary>Sets a repository secret, replacing any earlier value.</summary>
    /// <param name="name">The name of the secret.</param>
    /// <param name="value">The secret value. It is never put in an argument list or an error message.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>A task that completes when GitHub has the secret.</returns>
    /// <exception cref="InvalidOperationException">GitHub did not accept the secret.</exception>
    Task SetSecretAsync(string name, string value, CancellationToken cancellationToken);

    /// <summary>Sets a repository variable, replacing any earlier value.</summary>
    /// <param name="name">The name of the variable.</param>
    /// <param name="value">The value of the variable.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>A task that completes when GitHub has the variable.</returns>
    /// <exception cref="InvalidOperationException">GitHub did not accept the variable.</exception>
    Task SetVariableAsync(string name, string value, CancellationToken cancellationToken);
}
