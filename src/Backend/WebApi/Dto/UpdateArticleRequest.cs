namespace WebApi.Dto;

public record UpdateArticleRequest(string ArticleId,
    string Title,
    Dictionary<string, object> Content,
    IEnumerable<string> Tags);
