using Application.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Domain.Utils;
using Domain.Utils.Errors;
using MediatR;

namespace Application.Users.Commands;

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

        int likes = type!.Value == RatingType.Like
                ? article.Likes + 1
                : article.Likes;

        int dislikes = type!.Value == RatingType.Dislike
            ? article.Dislikes + 1
            : article.Dislikes;
        var updateInDocumentsTask = _articlesRepository.UpdateArticleRating(article.Id, likes, dislikes);
        Task<Result> updateInRatingsTask = rating is null
            ? _ratingsRepository.AddRating(article.Id, request.CallerId, type!.Value, cancellationToken) // add new
            : _ratingsRepository.UpdateRating(article.Id, request.CallerId, type!.Value, cancellationToken); // update current

        await Task.WhenAll(updateInDocumentsTask, updateInRatingsTask);
        return Result.Success();
    }
}
