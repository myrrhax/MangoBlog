using Domain.Entities;

namespace Application.Dto.Articles;

public record CreateArticleDto(string Title, 
    Dictionary<string, object> Content, 
    Guid CreatorId, 
    IEnumerable<Tag> Tags);
