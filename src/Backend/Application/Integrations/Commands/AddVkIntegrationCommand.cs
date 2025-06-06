using Application.Abstractions;
using Application.Dto;
using Application.Extentions;
using Domain.Entities;
using Domain.Enums;
using Domain.Utils;
using Domain.Utils.Errors;
using MediatR;

namespace Application.Integrations.Commands;

public record AddVkIntegrationCommand(Guid CallerId, 
    string ApiToken) : IRequest<Result<IntegrationDto>>;

public class AddVkIntegrationCommandHandler : IRequestHandler<AddVkIntegrationCommand, Result<IntegrationDto>>
{
    private readonly IVkApiService _vkApiService;
    private readonly IIntegrationRepository _integrationRepository;
    private readonly IUserRepository _userRepository;

    public AddVkIntegrationCommandHandler(IVkApiService tokenChecker,
        IIntegrationRepository integrationRepository,
        IUserRepository userRepository)
    {
        _vkApiService = tokenChecker;
        _integrationRepository = integrationRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<IntegrationDto>> Handle(AddVkIntegrationCommand request, CancellationToken cancellationToken)
    {
        ApplicationUser? user = await _userRepository.GetUserById(request.CallerId, cancellationToken);
        if (user is null)
            return Result.Failure<IntegrationDto>(new UserNotFound());

        Result tokenPermissionsResult = await _vkApiService.CheckTokenPermissions(request.ApiToken);
        if (tokenPermissionsResult.IsFailure)
            return Result.Failure<IntegrationDto>(tokenPermissionsResult.Error);

        var groupInfo = await _vkApiService.GetTokenGroupId(request.ApiToken);
        if (groupInfo.IsFailure)
            return Result.Failure<IntegrationDto>(groupInfo.Error);

        string groupId = groupInfo.Value.groupId;
        string groupName = groupInfo.Value.groupName;
        UserIntegration? integrationSearch = await _integrationRepository.GetIntegrationGroupId(IntegrationType.VKontakte, groupId, cancellationToken);

        if (integrationSearch is not null)
            return Result.Failure<IntegrationDto>(new IntegrationAlreadyExists(groupId, IntegrationType.VKontakte));

        Integration integration = await _integrationRepository.GetIntegration(IntegrationType.VKontakte, cancellationToken);
        var userIntegration = new UserIntegration(integration,
            user,
            apiToken: request.ApiToken,
            roomId: groupId,
            roomName: groupName,
            isConfirmed: true);
        Result insertionResult = await _integrationRepository.AddIntegration(userIntegration, cancellationToken);

        return insertionResult switch
        {
            { IsSuccess: true } => Result.Success(userIntegration.MapToDto()),
            _ => Result.Failure<IntegrationDto>(insertionResult.Error)
        };
    }
}
