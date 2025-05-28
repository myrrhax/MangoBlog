namespace Domain.Utils;

public abstract class Error
{
    public required string Message { get; init; }

    protected Error(string message)
    {
        Message = message;
    }
}