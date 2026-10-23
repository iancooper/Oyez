using Oyez.Core.Announcements;

namespace Oyez.Core;

/// <summary>
/// Either a value, or the <see cref="ValidationError"/>s that prevented one.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public sealed class Result<T>
{
    private readonly T? _value;

    private Result(T? value, IReadOnlyList<ValidationError> errors)
    {
        _value = value;
        Errors = errors;
    }

    /// <summary>Creates a successful result.</summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="Result{T}"/> holding <paramref name="value"/>.</returns>
    public static Result<T> Success(T value) => new(value, []);

    /// <summary>Creates a failed result.</summary>
    /// <param name="errors">The <see cref="ValidationError"/>s. There must be at least one.</param>
    /// <returns>A <see cref="Result{T}"/> holding <paramref name="errors"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errors"/> is empty.</exception>
    public static Result<T> Failure(IReadOnlyList<ValidationError> errors) =>
        errors.Count > 0
            ? new(default, errors)
            : throw new ArgumentException("A failure needs at least one error.", nameof(errors));

    /// <summary>Gets whether the result holds a value.</summary>
    /// <value><c>true</c> if there are no <see cref="Errors"/>.</value>
    public bool IsSuccess => Errors.Count == 0;

    /// <summary>Gets the problems that prevented a value.</summary>
    /// <value>The <see cref="ValidationError"/>s; empty when <see cref="IsSuccess"/> is <c>true</c>.</value>
    public IReadOnlyList<ValidationError> Errors { get; }

    /// <summary>Gets the value.</summary>
    /// <value>The value of type <typeparamref name="T"/>.</value>
    /// <exception cref="InvalidOperationException">Thrown when the result has failed.</exception>
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("A failed result has no value.");
}
