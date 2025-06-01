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

    public async Task<Result<Article>> CreateArticle(CreateArticleDto dto, CancellationToken cancellationToken)
    {
        ArticleDocument document = new ArticleDocument
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Title = dto.Title,
            CreatorId = dto.CreatorId,
            CreationDate = DateTime.UtcNow,
            Tags = dto.Tags.ToList(),
            Content = dto.Content.ToBsonDocument(),
        };
        try
        {
            await _articles.InsertOneAsync(document);
            _logger.LogInformation("New article was inserted with id: {}", document.Id);

            return Result.Success<Article>(document);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred article insertion (article id: {}).\nError: {}\nStack trace: {}",
                document.Id, ex.Message, ex.StackTrace);

            return Result.Failure<Article>(new DatabaseInteractionError("Failed to insert document"));
        }
    }
}
