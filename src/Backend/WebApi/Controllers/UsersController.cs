using Application.Dto;
using Application.Users.Commands;
using Domain.Utils;
using Domain.Utils.Errors;
using Infrastructure.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
            RefreshToken = result.Value!.RefreshToken;

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
            RefreshToken = result.Value!.RefreshToken;

            return Ok(result.Value!);
        }

        return result.Error switch
        {
            UserNotFound or InvalidLoginOrPassword => NotFound(result.Error),
            _ => BadRequest(result.Error)
        };
    }

    private string RefreshToken
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
