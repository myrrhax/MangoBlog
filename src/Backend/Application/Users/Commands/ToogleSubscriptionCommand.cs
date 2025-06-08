using Application.Abstractions;
using Application.Extentions;
using Domain.Enums;
using Domain.Utils;
using Domain.Utils.Errors;
using FluentValidation;
using MediatR;

namespace Application.Users.Commands;

public record ToogleSubscriptionCommand(Guid CallerId, 
    Guid UserId, 
    string SubscriptionType) : IRequest<Result>;

public class ToogleSubscriptionCommandHandler : IRequestHandler<ToogleSubscriptionCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<ToogleSubscriptionCommand> _validator;

    public ToogleSubscriptionCommandHandler(IUserRepository userRepository, 
        IValidator<ToogleSubscriptionCommand> validator)
    {
        _userRepository = userRepository;
        _validator = validator;
    }

    public async Task<Result> Handle(ToogleSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);

        if (validationResult.Errors.Any())
        {
            var errors = validationResult.Errors.ToErrorsDictionary();
            return Result.Failure(new ApplicationValidationError(errors));
        }

        SubscriptionType type = StringParsing.ParseSubscription(request.SubscriptionType);

        if (type == SubscriptionType.Subscribe)
            return await _userRepository.AddSubscription(request.CallerId, request.UserId, cancellationToken);

        return await _userRepository.RemoveSubscription(request.CallerId, request.UserId, cancellationToken);
    }
}
