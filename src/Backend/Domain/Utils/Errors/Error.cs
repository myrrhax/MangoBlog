namespace Domain.Utils.Errors;

public abstract class Error
{
    public string Message { get; init; }

    protected Error(string message)
    {
        Message = message;
    }
}

public class EmailOrLoginAlreadyTaken(string login, string email)
    : Error($"Email: {email} or login: {login} already taken");
public class UnableToWriteTokens() : Error("Unable to write token");
public class UserNotFound() : Error("User not found");
public class InvalidLoginOrPassword() : Error("Invalid login or password");
public class PasswordsDoesntMatch() : Error("Passwords doesn't match");