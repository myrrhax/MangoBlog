using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Infrastructure.MongoModels;

internal class PublicationDocument
{
    [BsonId]
    public ObjectId PublicationId { get; set; }
    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<Guid> MediaIds { get; set; } = [];
    public DateTime CreationDate { get; set; }
    public DateTime PublicationTime { get; set; }
    public bool IsPublished { get; set; }
}
