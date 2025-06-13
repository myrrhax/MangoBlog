using Application.Abstractions;
using Application.Dto;
using Application.Extentions;
using Domain.Entities;
using MediatR;

namespace Application.Publications.Queries;

public record GetMyPublicationsQuery(Guid UserId) : IRequest<IEnumerable<PublicationDto>>;

public class GetMyPublicationsQueryHandler : IRequestHandler<GetMyPublicationsQuery, IEnumerable<PublicationDto>>
{
    private readonly IIntegrationRepository _integrationRepository;
    private readonly IPublicationsRepository _publicationsRepository;

    public GetMyPublicationsQueryHandler(IIntegrationRepository integrationRepository,
        IPublicationsRepository publicationsRepository)
    {
        _integrationRepository = integrationRepository;
        _publicationsRepository = publicationsRepository;
    }

    public async Task<IEnumerable<PublicationDto>> Handle(GetMyPublicationsQuery request, CancellationToken cancellationToken)
    {
        var getPublicationsTask = _publicationsRepository.GetUserPublications(request.UserId);
        var getUserIntegrationTask = _integrationRepository.GetTelegramIntegration(request.UserId, cancellationToken);

        await Task.WhenAll(getPublicationsTask, getUserIntegrationTask);
        IEnumerable<Publication> publications = getPublicationsTask.Result;
        TelegramIntegration? integration = getUserIntegrationTask.Result;

        if (integration is null || !publications.Any())
            return Enumerable.Empty<PublicationDto>();

        var dict = integration.ConnectedChannels
            .ToDictionary(channel => channel.ChannelId, channel => channel.Name);
        
        return publications.Select(publication => publication.MapToDto(dict));
    }
}
