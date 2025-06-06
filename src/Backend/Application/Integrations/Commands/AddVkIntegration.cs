using Application.Dto;
using Domain.Utils;
using MediatR;

namespace Application.Integrations.Commands;

public record AddVkIntegration(Guid CallerId, 
    string ApiToken,
    string GroupId) : IRequest<Result<IntegrationDto>>;
