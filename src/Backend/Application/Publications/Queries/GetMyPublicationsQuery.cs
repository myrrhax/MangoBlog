using Application.Dto;
using MediatR;

namespace Application.Publications.Queries;

public record GetMyPublicationsQuery(Guid UserId) : IRequest<IEnumerable<PublicationDto>>;

public class 