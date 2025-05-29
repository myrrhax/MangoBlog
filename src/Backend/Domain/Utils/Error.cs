namespace Domain.Utils;

public abstract class Error
{
    public string Message { get; init; }

    protected Error(string message)
    {
        Message = message;
    }
}