namespace WebApi.Dto;

public record UpdateArticleRequest(string Id,
    string Title,
    Dictionary<string, object> Content,
    IEnumerable<string> Tags);
