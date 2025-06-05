using Application.MediFiles.Commands;
using Domain.Entities;
using Domain.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/media")]
public class MediaController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> LoadFile([FromForm] IFormFile file, [FromForm] bool isAvatar)
    {
        Guid userId = User.GetUserId()!.Value;
        string extention = Path.GetExtension(file.FileName).ToLower();

        using Stream stream = file.OpenReadStream();
        var command = new LoadFileCommand(userId, stream, extention, isAvatar);
        Result<string> loadingResult = await mediator.Send(command);

        return loadingResult.IsSuccess
            ? CreatedAtAction(nameof(GetFile), new { name = loadingResult.Value }, loadingResult.Value)
            : BadRequest(loadingResult.Error);
    }

    [HttpGet]
    [Route("name")]
    public async Task<IActionResult> GetFile([FromRoute] string name)
    {
        return Ok();
    }
}
