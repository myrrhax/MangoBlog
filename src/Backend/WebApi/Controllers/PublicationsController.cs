using Application.Dto;
using Application.Integrations.Commands;
using Application.Publications.Command;
using Domain.Utils;
using Domain.Utils.Errors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;

namespace WebApi.Controllers;

[ApiController]
[Route("api/publications")]
[Authorize]
public class PublicationsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Publish([FromBody] AddPublicationRequestDto dto)
    {
        Guid userId = User.GetUserId()!.Value;
        var command = new AddPublicationCommand(dto.Content,
            dto.MediaIds,
            userId,
            dto.PublicationDate);
        Result<PublicationDto> result = await mediator.Send(command);

        return result switch
        {
            { IsSuccess: true } => Ok(result.Value),
            { Error: UserNotFound or NoChannlesToPublish } => NotFound(result.Error),
            _ => BadRequest(result.Error)
        };
    }

    [HttpPost]
    [Authorize]
    [Route("confirm")]
    public async Task<IActionResult> ConfirmMessageSending([FromBody] ConfirmPublicationRequestDto dto)
    {
        Guid id = User.GetUserId()!.Value;
        var command = new ConfirmPublicationCommand(id, dto.PublicationId, dto.RoomId, dto.IntegrationType, dto.MessageId);
        Result res = await mediator.Send(command);

        return res switch
        {
            { IsSuccess: true } => Ok(),
            { Error: ConfrimationStatusIsNotFound } => NotFound(res.Error),
            _ => BadRequest(res.Error),
        };
    }
}
