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

    public Task<Result> ConfirmPublicationStatus(string PublicationId, string ChannelId, IntegrationType type)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> IsStatusUnconfirmed(Guid userId, string PublicationId, string ChannelId, IntegrationType type)
    {
        if (!ObjectId.TryParse(PublicationId, out ObjectId id)) 
        {
            return Result.Failure<bool>(new UnparsableId(PublicationId));
        }
        try
        {

            PublicationDocument? doc = await _publications.Find(publication => publication.PublicationId == id
                && publication.UserId == userId
                && publication.IntegrationPublishInfos.Where(publishInfo => publishInfo.IntegrationType == type)
                    .Select(publishInfo => publishInfo.PublishStatuses)
                    .Any(roomStatus => roomStatus.Any(status => status.RoomId == ChannelId && !status.IsPublished)))
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
