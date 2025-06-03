using Domain.Enums;

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
public class InvalidToken() : Error("Invalid or expired refresh token");
public class DatabaseInteractionError(string message) : Error($"Unable to interact with database. Error: {message}");
public class ArticleNotFound(string articleId) : Error($"Article with id: {articleId} is not found");
public class NoPermission(Guid userId) : Error($"User with id: {userId} has no permission for this");
public class RatingNotFound(string postId)
    : Error($"Rating for post ({postId}) is not found");

public class RatingAlreadyExists(Guid userId, string postId)
    : Error($"User {userId} already have same rating for post: {postId}");