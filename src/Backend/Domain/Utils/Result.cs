namespace Domain.Utils;

public class Result
{
    protected Error? _error;
    public bool IsSuccess { get; }
    public Error? Error
    {
        get
        {
            if (IsSuccess)
                throw new InvalidOperationException();
            return _error;
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
        => new Result(false, error);
}

public class Result<TValue> : Result where TValue : class
{
    private TValue? _value;
    public TValue? Value
    {
        get
        {
            if (!IsSuccess)
                throw new InvalidOperationException();
            return _value;
        }
    }
    protected Result(bool success, Error? error = null, TValue? value = null)
        : base(success, error) { }

    public static Result<TValue> Success(TValue value)
        => new Result<TValue>(true, value: value);

    public new static Result<TValue> Failure(Error error)
        => new Result<TValue>(false, error: error);
}
