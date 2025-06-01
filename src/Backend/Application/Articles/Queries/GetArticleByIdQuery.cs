using Application.Abstractions;
using Application.Dto.Articles;
using Application.Extentions;
using Domain.Entities;
using MediatR;

namespace Application.Articles.Queries;

public record GetArticleByIdQuery(string ArticleId) : IRequest<ArticleDto?>;

public class GetArticleByIdQueryHandler : IRequestHandler<GetArticleByIdQuery, ArticleDto?>
{
    private readonly IArticlesRepository _articlesRepository;
    private readonly IUserRepository _userRepository;

    public GetArticleByIdQueryHandler(IArticlesRepository articlesRepository, IUserRepository userRepository)
    {
        _articlesRepository = articlesRepository;
        _userRepository = userRepository;
    }

    public async Task<ArticleDto?> Handle(GetArticleByIdQuery request, CancellationToken cancellationToken)
    {
        Article? article = await _articlesRepository.GetArticleById(request.ArticleId);
        
        if (article is null)
            return null;

        ApplicationUser? creator = await _userRepository.GetUserById(article.CreatorId, cancellationToken);
        if (creator is null)
            return null;


    }
}