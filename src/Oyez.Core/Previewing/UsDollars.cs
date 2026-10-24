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

namespace Oyez.Core.Previewing;

/// <summary>
/// An amount of money in US dollars.
/// </summary>
/// <param name="Amount">The number of dollars.</param>
public readonly record struct UsDollars(decimal Amount)
{
    /// <summary>
    /// Adds two amounts.
    /// </summary>
    /// <param name="left">The first amount.</param>
    /// <param name="right">The second amount.</param>
    /// <returns>The sum of the two amounts.</returns>
    public static UsDollars operator +(UsDollars left, UsDollars right) => new(left.Amount + right.Amount);

    /// <summary>
    /// Formats the amount with a dollar sign and three decimal places, such as <c>$0.230</c>.
    /// </summary>
    /// <returns>The formatted amount.</returns>
    public override string ToString() => $"${Amount:0.000}";
}
