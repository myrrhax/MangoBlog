using Application.Dto;
using Domain.Utils;
using MediatR;

namespace Application.Integrations.Commands;

public record AddVkIntegrationCommand(Guid CallerId, 
    string ApiToken,
    string GroupId) : IRequest<Result<IntegrationDto>>;

public class AddVkIntegrationCommandHandler : IRequestHandler<AddVkIntegrationCommand, Result<IntegrationDto>>
{
    public Task<Result<IntegrationDto>> Handle(AddVkIntegrationCommand request, CancellationToken cancellationToken)
    {
        
    }
}
