using Domain.Enums;
using Domain.Utils;
using MediatR;

namespace Application.Users.Commands;

public record AddRatingToPostCommand(string PostId, 
    Guid CallerId, 
    RatingType Type) : IRequest<Result>;
