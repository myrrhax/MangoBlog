using Application.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Domain.Utils;
using Domain.Utils.Errors;
using MediatR;

namespace Application.Ratings.Commands;

public record AddRatingToPostCommand(string PostId, 
    Guid CallerId, 
    string RatingType) : IRequest<Result>;

public class AddRatingToPostCommandHandler : IRequestHandler<AddRatingToPostCommand, Result>
{
    private readonly IArticlesRepository _articlesRepository;
    private readonly IRatingsRepository _ratingsRepository;

    public AddRatingToPostCommandHandler(IArticlesRepository articlesRepository, IRatingsRepository ratingsRepository)
    {
        _articlesRepository = articlesRepository;
        _ratingsRepository = ratingsRepository;
    }

    public async Task<Result> Handle(AddRatingToPostCommand request, CancellationToken cancellationToken)
    {
        if (!StringParsing.TryParseRatingType(request.RatingType, out RatingType? type))
        {
            var error = ApplicationValidationError.ErrorFrom(nameof(request.RatingType),
                $"Unable to parse value: {request.RatingType}. Valid types: {{like, dislike}}");

            return Result.Failure(error);
        }

        Article? article = await _articlesRepository.GetArticleById(request.PostId);
        if (article is null || article.CreatorId == request.CallerId)
            return Result.Failure(new ArticleNotFound(request.PostId));

        RatingType? rating = await _ratingsRepository.GetUserRating(request.CallerId, request.PostId, cancellationToken);

        if (article is null)
            return Result.Failure(new ArticleNotFound(request.PostId));

        bool removeRating = rating is not null && rating == type;
        if (removeRating) // remove rating from article
        {
            Result removeResult = await RemoveRating(article.Id, request.CallerId, rating!.Value, cancellationToken);

            return removeResult;
        }

        // change rating or add new rating
        bool changeOld = rating is not null;

        var updateInDocumentsTask = _articlesRepository.PerformRatingChange(article.Id, type!.Value, changeOld);
        Task<Result> updateInRatingsTask = rating is null
            ? _ratingsRepository.AddRating(article.Id, request.CallerId, type!.Value, cancellationToken) // add new
            : _ratingsRepository.UpdateRating(article.Id, request.CallerId, type!.Value, cancellationToken); // update current

        await Task.WhenAll(updateInDocumentsTask, updateInRatingsTask);
        return Result.Success();
    }

    private async Task<Result> RemoveRating(string postId, Guid userId, RatingType type, CancellationToken cancellationToken)
    {
        var decrementTask = _articlesRepository.DecrementRatingFromArtcile(postId, type);
        var removeUserRating = _ratingsRepository.RemoveRatingFromPost(postId, userId, cancellationToken);

        await Task.WhenAll(decrementTask, removeUserRating);
        Result decrementResult = decrementTask.Result;
        Result removeUserResult = removeUserRating.Result;

        if (decrementResult.IsFailure)
            return decrementResult;

        if (removeUserResult.IsFailure)
            return removeUserResult;

        return Result.Success();
    }
}
