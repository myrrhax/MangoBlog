using Application.Abstractions;
using Application.Dto;
using Application.Extentions;
using Domain.Entities;
using Domain.Utils;
using MediatR;

namespace Application.Users.Queries;

public record AboutMeCommand(Guid CallerId) : IRequest<UserFullInfoDto?>;

public class AboutMeCommandHandler : IRequestHandler<AboutMeCommand, UserFullInfoDto?>
{
    private readonly IUserRepository _userRepository;

    public AboutMeCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserFullInfoDto?> Handle(AboutMeCommand request, CancellationToken cancellationToken)
    {
        ApplicationUser? entity = await _userRepository.GetUserById(request.CallerId, cancellationToken);

        if (entity is null)
            return null;

        return entity.MapToFullInfo();
    }
}
