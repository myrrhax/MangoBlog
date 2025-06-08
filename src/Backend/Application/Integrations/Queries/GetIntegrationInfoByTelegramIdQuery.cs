using Application.Abstractions;
using Application.Dto.Integrations;
using Application.Extentions;
using Domain.Entities;
using MediatR;

namespace Application.Integrations.Queries;

public record GetIntegrationInfoByTelegramIdQuery(string TelegramId) : IRequest<IntegrationDto?>;

public class GetIntegrationInfoByTelegramIdQueryHandler : IRequestHandler<GetIntegrationInfoByTelegramIdQuery, IntegrationDto?>
{
    private readonly IIntegrationRepository _integrationRepository;

    public GetIntegrationInfoByTelegramIdQueryHandler(IIntegrationRepository integrationRepository)
    {
        _integrationRepository = integrationRepository;
    }

    public async Task<IntegrationDto?> Handle(GetIntegrationInfoByTelegramIdQuery request, CancellationToken cancellationToken)
    {
        Integration? entity = await _integrationRepository.GetIntegrationByTelegramId(request.TelegramId, cancellationToken);

        return entity is null
            ? null
            : entity.MapToDto();
    }
}
