using Application.Abstractions;
using Domain.Utils;
using MediatR;

namespace Application.Users.Commands;

public record SubscribeCommand(Guid CallerId, 
    Guid UserId) : IRequest<Result>;

public class SubscribeCommandHandler : IRequestHandler<SubscribeCommand, Result>
{
    private readonly IUserRepository _userRepository;

    public SubscribeCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(SubscribeCommand request, CancellationToken cancellationToken)
    {
        return await _userRepository.AddSubscription(request.CallerId, request.UserId, cancellationToken);
    }
}
