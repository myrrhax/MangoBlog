using Application.Abstractions;
using Application.Dto.Articles;
using DnsClient.Internal;
using Domain.Entities;
using Domain.Utils;
using Domain.Utils.Errors;
using Infrastructure.MongoModels;
using Infrastructure.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Infrastructure.Implementation;

internal class ArticlesRepositoryImpl : IArticlesRepository
{
    private readonly IMongoCollection<ArticleDocument> _articles;
    private readonly ILogger<ArticlesRepositoryImpl> _logger;

    public ArticlesRepositoryImpl(IMongoClient client, IOptions<MongoConnectionConfig> config, ILogger<ArticlesRepositoryImpl> logger)
    {
        var database = client.GetDatabase(config.Value.DatabaseName);
        _articles = database.GetCollection<ArticleDocument>(MongoConnectionConfig.ArticlesCollectionName);
        _logger = logger;
    }

    public async Task<Result> CreateArticle(Article dto)
    {
        ArticleDocument document = dto.ToDocument();
        document.Id = ObjectId.GenerateNewId().ToString();
        try
        {
            await _articles.InsertOneAsync(document);
            _logger.LogInformation("New article was inserted with id: {}", document.Id);
            dto.Id = document.Id;
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred article insertion (article id: {}).\nError: {}\nStack trace: {}",
                document.Id, ex.Message, ex.StackTrace);

            return Result.Failure<Article>(new DatabaseInteractionError("Failed to insert document"));
        }
    }

    public async Task<Result> DeleteArtcile(string artcileId)
    {
        try
        {
            await _articles.FindOneAndDeleteAsync(article => article.Id == artcileId);
            _logger.LogInformation("Article with id: {} successfully deleted", artcileId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred deleting article with id: {}. Error: {}\nStack trace: {}", artcileId, ex.Message, ex.StackTrace);

            return Result.Failure(new DatabaseInteractionError($"Failed to delete article with id: {artcileId}"));
        }
    }

    public async Task<Article?> GetArticleById(string id)
    {
        return await _articles.Find(article => article.Id == id)
            .SingleAsync();
    }

    public async Task<IEnumerable<Article>> GetUserArticles(Guid userId)
    {
        IEnumerable<ArticleDocument> documents = await _articles.Find(article => article.CreatorId == userId)
            .ToListAsync();

        return documents.Select(document => document.MapToEntity());
    }

    public async Task<Result> ReplaceArticle(Article dto)
    {
        try
        {
            ArticleDocument document = dto.ToDocument();
            await _articles.FindOneAndReplaceAsync(article => article.Id == dto.Id,
                document);

            _logger.LogInformation("Article with id: {} was replaced with new document", dto.Id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred article replacement (articleId: {}). Error: {}", dto.Id, ex.Message);

            return Result.Failure(new DatabaseInteractionError("Failed to update article"));
        }
    }
}
