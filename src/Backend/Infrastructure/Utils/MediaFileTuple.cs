using Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Infrastructure.Utils;

internal class MediaFileTuple
{
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public MediaFileType Type { get; set; }
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public Guid MediaId { get; set; }
}
