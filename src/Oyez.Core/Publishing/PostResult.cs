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
/// What happened when a publisher tried to make a post.
/// </summary>
public abstract record PostResult
{
    /// <summary>The post was made.</summary>
    /// <param name="Ref">Where the post is on the platform.</param>
    public sealed record Posted(PostRef Ref) : PostResult;

    /// <summary>The platform definitely did not make the post.</summary>
    /// <param name="Reason">Why the post was refused.</param>
    public sealed record Rejected(string Reason) : PostResult;

    /// <summary>The post may or may not have been made.</summary>
    /// <param name="Reason">Why the outcome is unknown.</param>
    public sealed record Indeterminate(string Reason) : PostResult;
}
