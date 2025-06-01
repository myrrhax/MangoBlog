using Application.Abstractions;
using Application.Dto.Articles;
using Domain.Entities;
using MediatR;

namespace Application.Articles.Queries;

public record GetArticleByIdQuery(string ArticleId) : IRequest<ArticleDto?>;

public class GetArticleByIdQueryHandler : IRequestHandler<GetArticleByIdQuery, ArticleDto?>
{
    private readonly IArticlesRepository _articlesRepository;

    public GetArticleByIdQueryHandler(IArticlesRepository articlesRepository)
    {
        _articlesRepository = articlesRepository;
    }

    public Task<ArticleDto?> Handle(GetArticleByIdQuery request, CancellationToken cancellationToken)
    {
        Article? article = 
    }
}