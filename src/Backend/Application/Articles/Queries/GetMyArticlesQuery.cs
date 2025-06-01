using Application.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Articles.Queries;

public record GetMyArticlesQuery(Guid UserId) : IRequest<IEnumerable<Article>>;

public class GetMyArticlesQueryHandler : IRequestHandler<GetMyArticlesQuery, IEnumerable<Article>>
{
    private readonly IArticlesRepository _articlesRepository;

    public GetMyArticlesQueryHandler(IArticlesRepository articlesRepository)
    {
        _articlesRepository = articlesRepository;
    }

    public Task<IEnumerable<Article>> Handle(GetMyArticlesQuery request, CancellationToken cancellationToken)
    {
        
    }
}