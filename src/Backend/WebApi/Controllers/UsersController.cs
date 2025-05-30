﻿using Application.Dto;
using Application.Users.Commands;
using Domain.Utils;
using Domain.Utils.Errors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserCommand request)
    {
        Result<RegistrationResponse> result = await mediator.Send(request);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserCommand request)
    {
        Result<LoginResponse> result = await mediator.Send(request);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error switch
            {
                UserNotFound or InvalidLoginOrPassword => NotFound(result.Error),
                _ => BadRequest(result.Error)
            };
    }
}
