using Application.Abstractions;
using Domain.Entities;
using Domain.Utils;
using Domain.Utils.Errors;
using Infrastructure.MongoModels;
using Infrastructure.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
            publication.MediaIds,
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
}
