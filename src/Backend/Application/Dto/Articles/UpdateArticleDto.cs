using Domain.Entities;

namespace Application.Dto.Articles;

public record UpdateArticleDto(string ArticleId, 
    string Title, 
    Dictionary<string, object> Content, 
    IEnumerable<Tag> Tags);
