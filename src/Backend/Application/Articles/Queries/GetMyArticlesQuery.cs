using Application.Abstractions;
using Application.Dto.Articles;
using Application.Extentions;
using Domain.Entities;
using MediatR;

namespace Application.Articles.Queries;

public record GetMyArticlesQuery(Guid UserId) : IRequest<IEnumerable<ArticleDto>>;

public class GetMyArticlesQueryHandler : IRequestHandler<GetMyArticlesQuery, IEnumerable<ArticleDto>>
{
    private readonly IArticlesRepository _articlesRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRatingsRepository _ratingsRepository;

    public GetMyArticlesQueryHandler(IArticlesRepository articlesRepository, IUserRepository userRepository, IRatingsRepository ratingsRepository)
    {
        _articlesRepository = articlesRepository;
        _userRepository = userRepository;
        _ratingsRepository = ratingsRepository;
    }

    public async Task<IEnumerable<ArticleDto>> Handle(GetMyArticlesQuery request, CancellationToken cancellationToken)
    {
        Task<IEnumerable<Article>> getArticlesTask = _articlesRepository.GetUserArticles(request.UserId);
        Task<ApplicationUser?> getUserTask = _userRepository.GetUserById(request.UserId, cancellationToken);

        await Task.WhenAll(getArticlesTask, getUserTask);
        IEnumerable<Article> articles = getArticlesTask.Result;
        ApplicationUser? user = getUserTask.Result;

        if (!articles.Any() || user is null)
            return Enumerable.Empty<ArticleDto>();

        return articles.Select(article =>
        {
            return new ArticleDto(article.Id,
                user.MapToDto(),
                article.Title,
                article.Content,
                article.Tags,
                article.CreationDate,
                article.Likes,
                article.Dislikes,
                UserRating: null);
        });
    }
}