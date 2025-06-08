using Application.Abstractions;
using Application.Dto.Integrations;
using Application.Extentions;
using Domain.Entities;
using MediatR;

namespace Application.Integrations.Queries;

public record GetUserInfoByIntegrationCodeQuery(string IntegrationCode) : IRequest<IntegrationDto?>;

public class GetUserInfoByIntegrationCodeQueryHandler : IRequestHandler<GetUserInfoByIntegrationCodeQuery, IntegrationDto?>
{
    private readonly IIntegrationRepository _integrationRepository;

    public GetUserInfoByIntegrationCodeQueryHandler(IIntegrationRepository integrationRepository)
    {
        _integrationRepository = integrationRepository;
    }

    public async Task<IntegrationDto?> Handle(GetUserInfoByIntegrationCodeQuery request, CancellationToken cancellationToken)
    {
        Integration? entity = await _integrationRepository.GetIntegrationByCode(request.IntegrationCode, cancellationToken);

        return entity is null
            ? null
            : entity.MapToDto();
    }
}
