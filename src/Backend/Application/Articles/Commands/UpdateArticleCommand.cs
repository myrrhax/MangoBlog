using Application.Dto.Articles;
using Domain.Utils;
using MediatR;

namespace Application.Articles.Commands;

public record UpdateArticleCommand(string ArticleId, 
    Guid CallerId,
    string Title,
    Dictionary<string, object> Content, 
    IEnumerable<string> Tags) : IRequest<Result<ArticleDto>>;
