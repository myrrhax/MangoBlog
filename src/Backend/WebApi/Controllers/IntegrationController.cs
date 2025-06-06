using Application.Integrations.Commands;
using Domain.Utils;
using Domain.Utils.Errors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;

namespace WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/integrations")]
public class IntegrationController(IMediator mediator) : ControllerBase
{
    [Route("vk-integration")]
    [HttpPost]
    public async Task<IActionResult> AddVkIntegration([FromBody] AddVkIntegrationDto dto)
    {
        Guid userId = User.GetUserId()!.Value;
        var command = new AddVkIntegrationCommand(userId, dto.ApiToken);
        Result result = await mediator.Send(command);

        return result switch
        {
            { IsSuccess: true } => Ok(),
            _ => BadRequest(result.Error)
        };
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteIntegration([FromBody] DeleteIntegrationRequest dto)
    {
        Guid userId = User.GetUserId()!.Value;
        var command = new RemoveIntegrationCommand(userId, dto.IntegrationType, dto.RoomId);
        Result result = await mediator.Send(command);

        return result switch
        {
            { IsSuccess: true } => Ok(),
            { IsFailure: true, Error: IntegrationNotFound } => NotFound(),
            _ => BadRequest(result.Error)
        };
    }
}
