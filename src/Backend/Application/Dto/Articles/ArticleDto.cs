using Domain.Entities;

namespace Application.Dto.Articles;

public record ArticleDto(string Id, 
    UserDto creator, 
    Dictionary<string, object> Content, 
    IEnumerable<Tag> Tags,
    DateTime CreatioDate,
    int Likes,
    int Dislikes);
