using System.Threading.Channels;
using Application.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Domain.Utils;
using Domain.Utils.Errors;
using Infrastructure.MongoModels;
using Infrastructure.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Implementation;

internal class PublicationsRepositoryImpl : IPublicationsRepository
{
    private readonly IMongoCollection<PublicationDocument> _publications;
    private readonly ILogger<PublicationsRepositoryImpl> _logger;

    public PublicationsRepositoryImpl(IMongoClient client, 
        IOptions<MongoConnectionConfig> config,
        ILogger<PublicationsRepositoryImpl> logger)
    {
        var database = client.GetDatabase(config.Value.DatabaseName);
        _publications = database.GetCollection<PublicationDocument>(MongoConnectionConfig.PublicationsCollectionName);
        _logger = logger;
    }

    public async Task<Result> AddPublication(Publication publication)
    {
        var integrationsInfoDocument = publication.IntegrationPublishInfos
            .Select(info => info.MapToDocument())
            .ToList();
        var document = new PublicationDocument(publication.UserId,
            publication.Content,
            publication.MediaFiles.Select(media => media.MapToMediaTuple()).ToList(),
            publication.CreationDate,
            integrationsInfoDocument,
            publication.PublicationTime);

        try
        {
            await _publications.InsertOneAsync(document);
            publication.PublicationId = document.PublicationId.ToString();

            _logger.LogInformation("New publication with id: {} successfully inserted", publication.PublicationId);
            return Result.Success();
        }
        catch(Exception ex)
        {
            _logger.LogError("An error occurred database interaction: {}", ex.Message);
            return Result.Failure(new DatabaseInteractionError($"Failed to insert publication to database"));
        }
    }

    public async Task<Result> ConfirmPublicationStatus(string publicationId, string channelId, string messageId, IntegrationType type)
    {
        if (!ObjectId.TryParse(publicationId, out ObjectId id))
        {
            return Result.Failure<bool>(new UnparsableId(publicationId));
        }

        try
        {
            var filter = Builders<PublicationDocument>.Filter.And(
                Builders<PublicationDocument>.Filter.Eq(x => x.PublicationId, id),
                Builders<PublicationDocument>.Filter.ElemMatch(x => x.IntegrationPublishInfos,
                    info => info.IntegrationType == type &&
                            info.PublishStatuses.Any(status => status.RoomId == channelId && !status.IsPublished))
            );

            var update = Builders<PublicationDocument>.Update
                .Set("IntegrationPublishInfos.$[info].PublishStatuses.$[status].IsPublished", true)
                .Set("IntegrationPublishInfos.$[info].PublishStatuses.$[status].MessageId", messageId);

            var arrayFilters = new List<ArrayFilterDefinition>
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("info.IntegrationType", type.ToString())),
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument
                {
                    { "status.RoomId", channelId },
                    { "status.IsPublished", false }
                })
            };

            var result = await _publications.UpdateOneAsync(filter, update, new UpdateOptions { ArrayFilters = arrayFilters });

            if (result.ModifiedCount == 0)
            {
                return Result.Failure<bool>(new ConfrimationStatusIsNotFound(publicationId, channelId));
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update confirmation status. Error: {}", ex.Message);
            return Result.Failure(new DatabaseInteractionError("Failed to update confirmation status"));
        }
    }

    public async Task<Result> DeleteIntegrationPublicationsInfo(Guid userId, IntegrationType type)
    {
        try
        {
            // 1. Удаляем полностью те документы, где у пользователя единственный IntegrationType == Telegram
            FilterDefinition<PublicationDocument> deleteFilter = Builders<PublicationDocument>.Filter.And(
                Builders<PublicationDocument>.Filter.Eq(x => x.UserId, userId),
                Builders<PublicationDocument>.Filter.Size(x => x.IntegrationPublishInfos, 1),
                Builders<PublicationDocument>.Filter.Eq("IntegrationPublishInfos.0.IntegrationType", IntegrationType.Telegram)
            );
            await _publications.DeleteManyAsync(deleteFilter);

            // 2. В оставшихся документах удаляем IntegrationPublishInfos с IntegrationType == Telegram
            FilterDefinition<PublicationDocument> updateFilter = Builders<PublicationDocument>.Filter.And(
                Builders<PublicationDocument>.Filter.Eq(x => x.UserId, userId),
                Builders<PublicationDocument>.Filter.ElemMatch(x => x.IntegrationPublishInfos,
                    Builders<IntegrationPublicationInfoDocument>.Filter.Eq(x => x.IntegrationType, IntegrationType.Telegram))
            );

            UpdateDefinition<PublicationDocument> update = Builders<PublicationDocument>.Update.PullFilter(x => x.IntegrationPublishInfos,
                Builders<IntegrationPublicationInfoDocument>.Filter.Eq(x => x.IntegrationType, IntegrationType.Telegram));

            await _publications.UpdateManyAsync(updateFilter, update);
            _logger.LogInformation("Successfully deleted publications of type: {} for user: {}", type.ToString(),
                userId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred deleting publications of type: {} for user: {}. Error: {}", type.ToString(),
                userId, ex.Message);

            return Result.Failure(new DatabaseInteractionError("Failed to delete publications"));
        }
    }

    public async Task<Publication?> GetPublicationById(string id)
    {
        if (!ObjectId.TryParse(id, out ObjectId documentId))
            return null;

        var document = await _publications.Find(doc => doc.PublicationId == documentId)
            .SingleOrDefaultAsync();

        return document?.MapToEntity();
    }

    public async Task<IEnumerable<Publication>> GetUserPublications(Guid userId)
    {
        IEnumerable<PublicationDocument> documents = await _publications.Find(publication => publication.UserId == userId)
            .ToListAsync();

        return documents.Select(document => document.MapToEntity());
    }

    public async Task<Result> IsStatusUnconfirmed(Guid userId, string PublicationId, string ChannelId, IntegrationType type)
    {
        if (!ObjectId.TryParse(PublicationId, out ObjectId id)) 
        {
            return Result.Failure<bool>(new UnparsableId(PublicationId));
        }
        try
        {

            PublicationDocument? doc = await _publications.Find(publication =>
                    publication.PublicationId == id
                    && publication.UserId == userId
                    && publication.IntegrationPublishInfos.Any(publishInfo =>
                        publishInfo.IntegrationType == type
                        && publishInfo.PublishStatuses.Any(status =>
                            status.RoomId == ChannelId && !status.IsPublished
                        )
                    )
                )
                .SingleOrDefaultAsync();

            return doc is null
                ? Result.Failure<bool>(new ConfrimationStatusIsNotFound(PublicationId, ChannelId))
                : Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to load infromation about confirmation. Error: {}", ex.Message);

            return Result.Failure(new DatabaseInteractionError("Failed to load information"));
        }
    }
}
