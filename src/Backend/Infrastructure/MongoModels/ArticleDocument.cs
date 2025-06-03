using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Infrastructure.MongoModels;

internal class ArticleDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    
    public string Title { get; set; } = string.Empty;
    public BsonDocument Content { get; set; } = null!;
    
    [BsonRepresentation(BsonType.String)]
    public Guid CreatorId { get; set; }
    public ICollection<string> Tags { get; set; } = [];
    public DateTime CreationDate { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }


    public static implicit operator Article(ArticleDocument document)
    {
        return document.MapToEntity();
    }
}
