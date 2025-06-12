using Domain.Entities;
using Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Infrastructure.MongoModels;

internal class IntegrationPublicationInfoDocument
{
    [BsonRepresentation(BsonType.String)]
    public IntegrationType IntegrationType { get; set; }
    public List<RoomPublishStatus> PublishStatuses { get; set; } = [];
}
