using Domain.Entities;

namespace Application.Dto.Articles;

public record CreateArticleDto(string Title, 
    Dictionary<string, dynamic> Content, 
    Guid CreatorId, 
    IEnumerable<Tag> Tags);
