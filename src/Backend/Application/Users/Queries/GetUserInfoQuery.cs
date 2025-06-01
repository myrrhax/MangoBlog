using Application.Abstractions;
using Application.Dto;
using Application.Extentions;
using Domain.Entities;
using MediatR;

namespace Application.Users.Queries;

public record GetUserInfoQuery(Guid UserId) : IRequest<UserDto?>;

public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, UserDto?>
{
    private readonly IUserRepository _userRepository;

    public GetUserInfoQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
    {
        ApplicationUser? user = await _userRepository.GetUserById(request.UserId, cancellationToken);

        if (user is null)
            return null;

        return user!.MapToDto();
    }
}
