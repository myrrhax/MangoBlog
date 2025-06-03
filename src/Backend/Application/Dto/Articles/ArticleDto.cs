using Domain.Entities;
using Domain.Enums;

namespace Application.Dto.Articles;

public record ArticleDto(string Id, 
    UserDto? Creator, 
    string Title,
    Dictionary<string, object> Content, 
    IEnumerable<Tag> Tags,
    DateTime CreatioDate,
    int Likes,
    int Dislikes,
    RatingType? UserRating = null);
