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

using System.Security.Cryptography;
using System.Text;

namespace Oyez.Core.Channels;

/// <summary>
/// A fingerprint of the post texts in a <see cref="Rendering"/>.
/// </summary>
/// <param name="Value">The hash, in the form <c>sha256:</c> followed by lower-case hexadecimal.</param>
public readonly record struct RenderingHash(string Value)
{
    private const string PREFIX = "sha256:";
    private const int FINGERPRINT_LENGTH = 12;

    internal string Fingerprint => Value[PREFIX.Length..][..FINGERPRINT_LENGTH];

    internal static RenderingHash Of(IEnumerable<string> postTexts) =>
        new($"{PREFIX}{Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(string.Join('\0', postTexts))))}");

    /// <summary>Returns the hash as text.</summary>
    /// <returns>The <see cref="Value"/>.</returns>
    public override string ToString() => Value;
}
