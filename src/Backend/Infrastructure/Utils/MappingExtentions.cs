using System.Reflection.Metadata;
using Domain.Entities;
using Infrastructure.MongoModels;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Infrastructure;

internal static class MappingExtentions
{
    public static Article MapToEntity(this ArticleDocument document)
    {
        return new Article(document.Id,
            document.Title,
            document.Content.ToDictionary(),
            document.CreatorId,
            document.Tags,
            document.CreationDate);
    }

    public static ArticleDocument ToDocument(this Article article)
    {
        return new ArticleDocument
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Title = article.Title,
            CreatorId = article.CreatorId,
            CreationDate = article.CreationDate,
            Tags = article.Tags.ToList(),
            Content = BsonDocument.Parse(JsonConvert.SerializeObject(article.Content)),
        };
    }
}
