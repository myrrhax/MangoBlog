namespace WebApi.Dto;

public record CreateArticleRequest(string Title, 
    Dictionary<string, object> Content, 
    IEnumerable<string> Tags,
    Guid? CoverImageId);
