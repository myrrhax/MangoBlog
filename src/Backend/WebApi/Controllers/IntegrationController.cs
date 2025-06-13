﻿using Application.Dto;
using Application.Dto.Integrations;
using Application.Integrations.Commands;
using Application.Integrations.Queries;
using Domain.Utils;
using Domain.Utils.Errors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;

namespace WebApi.Controllers;

[ApiController]
[Route("api/integrations")]
public class IntegrationController(IMediator mediator) : ControllerBase
{
    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteIntegration([FromBody] DeleteIntegrationRequest dto)
    {
        Guid userId = User.GetUserId()!.Value;
        var command = new RemoveIntegrationCommand(userId, dto.IntegrationType);
        Result result = await mediator.Send(command);

        return result switch
        {
            { IsSuccess: true } => Ok(),
            { IsFailure: true, Error: IntegrationNotFound } => NotFound(),
            _ => BadRequest(result.Error)
        };
    }

    [HttpPost]
    [Route("tg")]
    [Authorize]
    public async Task<IActionResult> AddTelegramIntegration()
    {
        Guid userId = User.GetUserId()!.Value;
        var command = new AddTelegramIntegrationCommand(userId);
        Result result = await mediator.Send(command);

        return result.IsSuccess
            ? Ok()
            : BadRequest(result.Error);
    }

    [HttpPost]
    [Route("tg/confirm")]
    public async Task<IActionResult> ConfirmTelegramIntegration([FromBody] ConfrimTelegramIntegrationRequest request)
    {
        var command = new ConfirmTelegramIntegrationCommand(request.IntegrationCode, request.TelegramId);
        Result<ConfirmIntegrationResponse> result = await mediator.Send(command);

        return result switch
        {
            { IsSuccess: true } => Ok(result.Value),
            { IsFailure: true, Error: IntegrationNotFound } => NotFound(),
            _ => BadRequest(result.Error)
        };
    }

    [HttpGet]
    [Route("tg/{tgId}")]
    [Authorize]
    public async Task<IActionResult> GetIntegrationInfoByTelegramId([FromRoute] string tgId)
    {
        var query = new GetIntegrationInfoByTelegramIdQuery(tgId);
        IntegrationDto? dto = await mediator.Send(query);

        return dto is null
            ? NotFound()
            : Ok(dto);
    }

    [HttpPost]
    [Authorize]
    [Route("tg/add-channel")]
    public async Task<IActionResult> AttachChannelToUser([FromBody] AttachTelegramChannelDto dto)
    {
        Guid id = User.GetUserId()!.Value;
        var command = new AddTelegramChatToIntegrationCommand(id, dto.ChatId, dto.ChatName);
        Result result = await mediator.Send(command);

        return result.IsSuccess
            ? Ok()
            : NotFound();
    }

    [HttpPost]
    [Authorize]
    [Route("confirm")]
    public async Task<IActionResult> ConfirmMessageSending([FromBody] ConfirmPublicationRequestDto dto)
    {
        Guid id = User.GetUserId()!.Value;
        var command = new ConfirmPublicationCommand(id, dto.PublicationId, dto.RoomId, dto.IntegrationType);
        Result res = await mediator.Send(command);

        return res switch
        {
            { IsSuccess: true } => Ok(dto),
            { Error: ConfrimationStatusIsNotFound } => NotFound(res.Error),
            _ => BadRequest(res.Error),
        };
    }
}
