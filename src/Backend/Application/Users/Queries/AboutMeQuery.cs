using Application.Abstractions;
using Application.Dto;
using Application.Extentions;
using Domain.Entities;
using Domain.Utils;
using MediatR;

namespace Application.Users.Queries;

public record AboutMeQuery(Guid CallerId) : IRequest<UserFullInfoDto?>;

public class AboutMeQueryHandler : IRequestHandler<AboutMeQuery, UserFullInfoDto?>
{
    private readonly IUserRepository _userRepository;

    public AboutMeQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserFullInfoDto?> Handle(AboutMeQuery request, CancellationToken cancellationToken)
    {
        ApplicationUser? entity = await _userRepository.GetUserById(request.CallerId, cancellationToken);

        if (entity is null)
            return null;

        return entity.MapToFullInfo();
    }
}
