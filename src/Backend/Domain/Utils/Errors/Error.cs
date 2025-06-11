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

public class InvalidFileExtention(string extention, string[] validExtentions)
    : Error($"Cannot load file with extention type: {extention}. Valid extention type: {string.Join(", ", validExtentions)}");

public class FailedToLoadFile(string message)
    : Error($"An error occurred loading file to server: {message}");

public class SubscriptionAlreadyExists(Guid subscriberId, Guid channelId)
    : Error($"User {subscriberId} already subscribed to channel {channelId}");

public class SubscriptionNotFound(Guid subscriberId, Guid channelId)
    : Error($"Subscription ({subscriberId}, {channelId}) is not found");

public class MediaNotFound(Guid mediaId)
    : Error($"Media file with id {mediaId} is not found");

public class InvalidMediaFormat(Guid mediaId, MediaFileType type)
    : Error($"Media file {mediaId} have invalid format: {type.ToString()}");

public class MediaIsNotAvatar()
    : Error($"Media file must be an avatar");

public class InvalidApiToken() : Error("Invalid token");

public class IntegrationAlreadyExists(string type, Guid userId)
    : Error($"Integration {type} for user: {userId} already exists");

public class IntegrationNotFound()
    : Error($"Integration not found");

public class ApiTokenHasNoPermission()
    : Error("Api token doesn't have permissions for publications");

public class VkGroupNotFound()
    : Error("Vk group is not found");

public class NoChannlesToPublish(Guid userId)
    : Error($"User {userId} has no linked channels to publish");