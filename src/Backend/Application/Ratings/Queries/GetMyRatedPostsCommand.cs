using Application.Abstractions;
using Application.Dto.Articles;
using Application.Extentions;
using Domain.Entities;
using MediatR;

namespace Application.Ratings.Queries;

public record GetMyRatedPostsCommand(Guid CallerId) : IRequest<IEnumerable<ArticleDto>>;

public class GetMyRatedPostsCommandHandler : IRequestHandler<GetMyRatedPostsCommand, IEnumerable<ArticleDto>>
{
    private readonly IRatingsRepository _ratingsRepository;
    private readonly IArticlesRepository _articlesRepository;
    private readonly IUserRepository _userRepository;

    public GetMyRatedPostsCommandHandler(IRatingsRepository ratingsRepository, 
        IArticlesRepository articlesRepository, 
        IUserRepository userRepository)
    {
        _ratingsRepository = ratingsRepository;
        _articlesRepository = articlesRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<ArticleDto>> Handle(GetMyRatedPostsCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<Rating> userRatings = await _ratingsRepository.GetUserRatings(request.CallerId, cancellationToken);

        if (!userRatings.Any())
        {
            return Enumerable.Empty<ArticleDto>();
        }

        IEnumerable<string> articleIds = userRatings.Select(rating => rating.ArticleId);
        IEnumerable<Article> articles = await _articlesRepository.LoadArticles(articleIds);
        Dictionary<string, Guid> articleAuthorsDictionary = articles.ToDictionary(article => article.Id, 
            article => article.CreatorId);
        Dictionary<string, ApplicationUser?> creators = await _userRepository.LoadAuthors(articleAuthorsDictionary, cancellationToken);

        return articles.Select(article =>
        {
            Rating userRating = userRatings.First(rating => rating.ArticleId == article.Id);
            creators.TryGetValue(article.Id, out ApplicationUser? creator);
            return article.MapToDto(creator, userRating.RatingType);
        });
    }
}