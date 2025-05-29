namespace Domain.Utils;

public class Result
{
    protected readonly Error? _error;
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public Error Error
    {
        get
        {
            if (IsSuccess)
                throw new InvalidOperationException("Cannot access Error when value successfull");
            return _error!;
        }
    }

    protected Result(bool success, Error? error = null)
    {
        IsSuccess = success;
        _error = error;
    }

    public static Result Success()
        => new Result(true);

    public static Result Failure(Error error)
        => new Result(false, error ?? throw new ArgumentNullException(nameof(error)));

    public static Result<TValue> Success<TValue>(TValue value)
        => new(true, value: value ?? throw new ArgumentNullException(nameof(value)));

    public static Result<TValue> Failure<TValue>(Error error)
        => new(false, error: error ?? throw new ArgumentNullException(nameof(error)));
}

public class Result<TValue> : Result
{
    protected readonly TValue? _value;
    public TValue? Value
    {
        get
        {
            if (!IsSuccess)
                throw new InvalidOperationException("Cannot access Value when the result is failure");
            return _value;
        }
    }
    internal Result(bool success, Error? error = null)
        : base(success, error) { }

    internal Result(bool success, TValue value)
        : base(success, null)
    {
        _value = value;
    }
}
