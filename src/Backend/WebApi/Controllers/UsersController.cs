﻿using Application.Dto;
using Application.Users.Commands;
using Application.Users.Queries;
using Domain.Utils;
using Domain.Utils.Errors;
using Infrastructure.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Dto;

namespace WebApi.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IMediator mediator, IOptions<JwtConfig> jwtConfig) : ControllerBase
{
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserCommand request)
    {
        Result<RegistrationResponse> result = await mediator.Send(request);
        
        if (result.IsSuccess)
        {
            CurrentRefreshToken = result.Value!.RefreshToken;

            return Ok(result.Value!);
        }

        return BadRequest(result.Error);
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserCommand request)
    {
        Result<LoginResponse> result = await mediator.Send(request);

        if (result.IsSuccess)
        {
            CurrentRefreshToken = result.Value!.RefreshToken;

            return Ok(result.Value!);
        }

        return result.Error switch
        {
            UserNotFound or InvalidLoginOrPassword => NotFound(result.Error),
            _ => BadRequest(result.Error)
        };
    }

    [HttpPost]
    [Route("refresh")]
    public async Task<IActionResult> RefreshUser()
    {
        RefreshTokenCommand command = new(CurrentRefreshToken);
        Result<RefreshResponse> response = await mediator.Send(command);
        
        if (response.IsSuccess)
        {
            CurrentRefreshToken = response.Value!.RefreshToken;

            return Ok(response.Value!);
        }

        return NotFound(response.Error);
    }

    [Authorize]
    [HttpPost]
    [Route("logout")]
    public async Task<IActionResult> LogoutUser()
    {
        Guid? userId = User.GetUserId();
        if (userId is null || CurrentRefreshToken == string.Empty)
            return NotFound();

        LogoutUserCommand command = new(userId.Value, CurrentRefreshToken);
        Result response = await mediator.Send(command);

        if (response.IsSuccess)
        {
            RemoveToken();

            return Ok();
        }

        return NotFound();
    }

    [HttpGet]
    [Authorize]
    [Route("{id:guid}")]
    public async Task<IActionResult> AboutMe([FromRoute] Guid id)
    {
        GetUserInfoQuery query = new GetUserInfoQuery(id);
        UserDto? dto = await mediator.Send(query);

        return dto is null
            ? NotFound()
            : Ok(dto);
    }

    [HttpGet]
    [Authorize]
    [Route("me")]
    public async Task<IActionResult> AboutMe()
    {
        Guid userId = User.GetUserId() ?? Guid.Empty;
        AboutMeQuery query = new AboutMeQuery(userId);
        UserFullInfoDto? fullInfoDto = await mediator.Send(query);
        
        return fullInfoDto is null
            ? NotFound() 
            : Ok(fullInfoDto);
    }

    [HttpPost]
    [Authorize]
    [Route("subscription")]
    public async Task<IActionResult> ToggleSubscription([FromBody] SubscriptionRequestDto dto)
    {
        Guid callerId = User.GetUserId()!.Value;
        var command = new ToogleSubscriptionCommand(callerId, dto.ChannelId, dto.SubscriptionType);

        Result result = await mediator.Send(command);

        return result switch
        {
            { IsSuccess: true } => Ok(),
            { } error => error.Error switch
            {
                SubscriptionNotFound e => NotFound(e.Message),
                _ => BadRequest(error.Error)
            }
        };
    }

    [HttpPatch]
    [Authorize]
    [Route("avatar")]
    public async Task<IActionResult> ChangeAvatar([FromBody] UpdateAvatarRequestDto dto)
    {
        Guid userId = User.GetUserId()!.Value;
        var command = new UpdateAvatarCommand(userId, dto.AvatarId);
        Result result = await mediator.Send(command);

        return result switch
        {
            { IsSuccess: true } => Ok(),
            _ => result.Error switch
            {
                MediaNotFound => NotFound(),
                _ => BadRequest(result.Error)
            }
        };
    }

    private void RemoveToken()
    {
        HttpContext.Response.Cookies.Delete(jwtConfig.Value.CookieName);
    }

    private string CurrentRefreshToken
    {
        get
        {
            return HttpContext.Request.Cookies[jwtConfig.Value.CookieName] ?? string.Empty;
        }
        set
        {
            HttpContext.Response.Cookies.Append(jwtConfig.Value.CookieName, value 
                ?? throw new ArgumentNullException(nameof(value)), 
                new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                });
        }
    }
}
