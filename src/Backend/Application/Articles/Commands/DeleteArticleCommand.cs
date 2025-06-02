namespace Application.Articles.Commands;

public record DeleteArticleCommand(string ArticleId, Guid CallerId);
