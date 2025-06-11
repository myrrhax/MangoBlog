using MediatR;

namespace Application.Publications.Command;

public record AddPublicationCommand() : IRequest;
