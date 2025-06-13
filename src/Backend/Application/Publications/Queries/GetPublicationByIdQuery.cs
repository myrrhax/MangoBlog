using Application.Abstractions;
using Application.Dto;
using Application.Extentions;
using Domain.Entities;
using Domain.Utils;
using Domain.Utils.Errors;
using MediatR;

namespace Application.Publications.Queries;

public record GetPublicationByIdQuery(string PublicationId,
    Guid CallerId) : IRequest<Result<PublicationDto>>;

public class GetPublicationByIdQueryHandler : IRequestHandler<GetPublicationByIdQuery, Result<PublicationDto>>
{
    private readonly IPublicationsRepository _publicationsRepository;
    private readonly IIntegrationRepository _integrationRepository;

    public GetPublicationByIdQueryHandler(IPublicationsRepository publicationsRepository, IIntegrationRepository integrationRepository)
    {
        _publicationsRepository = publicationsRepository;
        _integrationRepository = integrationRepository;
    }

    public async Task<Result<PublicationDto>> Handle(GetPublicationByIdQuery request, CancellationToken cancellationToken)
    {
        Task<Publication?> getPublicationTask = _publicationsRepository.GetPublicationById(request.PublicationId);
        Task<TelegramIntegration?> getTgIntegrationTask = _integrationRepository.GetTelegramIntegration(request.CallerId, cancellationToken);

        await Task.WhenAll(getPublicationTask, getTgIntegrationTask);
        var publication = getPublicationTask.Result;
        var integrationInfo = getTgIntegrationTask.Result;

        if (publication is null)
            return Result.Failure<PublicationDto>(new PublicationNotFound(request.PublicationId));

        if (publication.UserId != request.CallerId)
            return Result.Failure<PublicationDto>(new NoPermission(request.CallerId));

        if (integrationInfo is null)
            return Result.Failure<PublicationDto>(new IntegrationNotFound());

        var channels = integrationInfo.ConnectedChannels
            .ToDictionary(info => info.ChannelId, info => info.Name);
        return Result.Success(publication.MapToDto(channels));
    }
}
