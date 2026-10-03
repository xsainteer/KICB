namespace Domain.Common;

/// <summary>
/// Outcome of an operation that produces a value or fails for an expected business reason.
/// </summary>
public sealed class Result<T> : Result
{
    private readonly T? _value;

    private Result(T value) : base(null) => _value = value;

    private Result(Error error) : base(error)
    {
    }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    /// <summary>
    /// Needed when <typeparamref name="T"/> is an interface (e.g. <c>IReadOnlyList&lt;X&gt;</c>):
    /// C# does not apply user-defined implicit conversions from interface types.
    /// </summary>
    public static Result<T> Success(T value) => new(value);

    public static implicit operator Result<T>(T value) => new(value);
    public static implicit operator Result<T>(Error error) => new(error);
}
