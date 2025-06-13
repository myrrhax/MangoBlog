using Domain.Entities;
using Infrastructure.Utils;
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
    public List<MediaFileTuple> Media { get; set; } = [];
    public DateTime CreationDate { get; set; }
    public DateTime? PublicationTime { get; set; }
    public List<IntegrationPublicationInfoDocument> IntegrationPublishInfos { get; set; } = [];

    public PublicationDocument(Guid userId,
        string content,
        List<MediaFileTuple> media,
        DateTime creationDate,
        List<IntegrationPublicationInfoDocument> infoDocuments,
        DateTime? publicationTime = null)
    {
        PublicationId = ObjectId.GenerateNewId();
        UserId = userId;
        Content = content;
        Media = media;
        CreationDate = creationDate;
        PublicationTime = publicationTime;
        IntegrationPublishInfos = infoDocuments;
    }

    public PublicationDocument()
    {
        
    }
}
