using Application.Abstractions;
using DnsClient.Internal;
using Domain.Entities;
using Domain.Enums;
using Domain.Utils;
using Domain.Utils.Errors;
using Infrastructure.MongoModels;
using Infrastructure.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
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

    public async Task<Result> DecrementRatingFromArtcile(string postId, RatingType type)
    {
        UpdateDefinition<ArticleDocument> definition = type == RatingType.Like
            ? Builders<ArticleDocument>.Update.Inc(article => article.Likes, -1) // раньше был лайк
            : Builders<ArticleDocument>.Update.Inc(article => article.Dislikes, -1); // раньше был дизлайк

        try
        {
            await _articles.UpdateOneAsync(article => article.Id == postId, definition);

            _logger.LogInformation("Rating successfully decremented for post: {}", postId);
            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError("Unable to decrement rating from post: {}. Error: {}", postId, e.Message);

            return Result.Failure(new DatabaseInteractionError("Unable to connect to database"));
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
        try 
        {
            return await _articles.Find(article => article.Id == id)
                .SingleAsync();
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<IEnumerable<Article>> GetArticles(IEnumerable<string> tags,
        string query,
        int page,
        int pageSize,
        SortType creationDateSort,
        SortType popularitySort,
        Guid? authorId = null)
    {
        var pipeline = new List<BsonDocument>();
        var matchFilter = new BsonDocument();
        if (!string.IsNullOrWhiteSpace(query))
        {
            matchFilter.Add("Title", new BsonDocument("$regex", query).Add("$options", "i"));
        }

        if (tags.Any())
        {
            var tagsArray = new BsonArray(tags);
            matchFilter.Add("Tags", new BsonDocument("$all", tagsArray));
        }

        if (authorId.HasValue)
        {
            matchFilter.Add("CreatorId", authorId.Value.ToString());
        }
        pipeline.Add(new BsonDocument("$match", matchFilter));

        // project
        if (popularitySort != SortType.None)
        {
            pipeline.Add(new BsonDocument("$addFields", new BsonDocument("totalVotes", new BsonDocument("$add", new BsonArray { "$Likes", "$Dislikes" }))));
        }

        var sortDefinition = new List<BsonElement>();
        if (popularitySort != SortType.None)
        {
            sortDefinition.Add(new BsonElement("totalVotes", popularitySort == SortType.Ascending ? 1 : -1));
        }
        if (creationDateSort != SortType.None)
        {
            sortDefinition.Add(new BsonElement("CreationDate", creationDateSort == SortType.Ascending ? 1 : -1));
        }

        if (sortDefinition.Any())
        {
            pipeline.Add(new BsonDocument("$sort", new BsonDocument(sortDefinition)));
        }

        pipeline.Add(new BsonDocument("$skip", (page - 1) * pageSize));
        pipeline.Add(new BsonDocument("$limit", pageSize));

        List<BsonDocument> rawDocs = await _articles.Aggregate<BsonDocument>(pipeline).ToListAsync();
        var articles = rawDocs.Select(doc =>
        {
            doc.Remove("totalVotes");
            var mapped = BsonSerializer.Deserialize<ArticleDocument>(doc);
            return mapped.MapToEntity();
        });
        return articles;
    }

    public async Task<IEnumerable<Article>> GetUserArticles(Guid userId)
    {
        IEnumerable<ArticleDocument> documents = await _articles.Find(article => article.CreatorId == userId)
            .ToListAsync();

        return documents.Select(document => document.MapToEntity());
    }

    public async Task<Result> PerformRatingChange(string postId, RatingType type, bool removeOld)
    {
        var updates = new List<UpdateDefinition<ArticleDocument>>();

        if (type == RatingType.Like)
            updates.Add(Builders<ArticleDocument>.Update.Inc(a => a.Likes, 1));
        else
            updates.Add(Builders<ArticleDocument>.Update.Inc(a => a.Dislikes, 1));

        if (removeOld)
        {
            if (type == RatingType.Like)
                updates.Add(Builders<ArticleDocument>.Update.Inc(a => a.Dislikes, -1));
            else
                updates.Add(Builders<ArticleDocument>.Update.Inc(a => a.Likes, -1));
        }

        var updateCommand = Builders<ArticleDocument>.Update.Combine(updates);

        try
        {
            await _articles.UpdateOneAsync(article => article.Id == postId, updateCommand);
            _logger.LogInformation("Post {} successfully updated", postId);

            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError("Unable to update post: {}. Error: {}", postId, e.Message);

            return Result.Failure(new DatabaseInteractionError("Unable to update post"));
        }
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
