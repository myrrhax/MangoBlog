using Application.Abstractions;
using Application.Dto.Articles;
using Application.Extentions;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Articles.Queries;

public record GetArticleByIdQuery(string ArticleId, Guid? UserId) : IRequest<ArticleDto?>;

public class GetArticleByIdQueryHandler : IRequestHandler<GetArticleByIdQuery, ArticleDto?>
{
    private readonly IArticlesRepository _articlesRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRatingsRepository _ratingsRepository;

    public GetArticleByIdQueryHandler(IArticlesRepository articlesRepository, IUserRepository userRepository, IRatingsRepository ratingsRepository)
    {
        _articlesRepository = articlesRepository;
        _userRepository = userRepository;
        _ratingsRepository = ratingsRepository;
    }

    public async Task<ArticleDto?> Handle(GetArticleByIdQuery request, CancellationToken cancellationToken)
    {
        Article? article = await _articlesRepository.GetArticleById(request.ArticleId);
        
        if (article is null)
            return null;

        ApplicationUser? creator = await _userRepository.GetUserById(article.CreatorId, cancellationToken);
        if (creator is null)
            return null;

        int likes = await _ratingsRepository.GetPostLikesCount(request.ArticleId, cancellationToken);
        int dislikes = await _ratingsRepository.GetPostDislikesCount(request.ArticleId, cancellationToken);
        RatingType? userReaction = request.UserId is null || request.UserId == creator.Id
            ? null
            : await _ratingsRepository.GetUserRating(request.UserId.Value, request.ArticleId, cancellationToken);

        return article.MapToDto(creator, likes, dislikes);
    }
}