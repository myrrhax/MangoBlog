using Application.MediFiles.Commands;
using Domain.Entities;
using Domain.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;

namespace WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/media")]
public class MediaController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> LoadFile([FromForm] MediaUploadDto dto)
    {
        if (dto.File is null || dto.File.Length == 0)
            return BadRequest();

        Guid userId = User.GetUserId()!.Value;
        string extention = Path.GetExtension(dto.File.FileName).ToLower();

        using Stream stream = dto.File.OpenReadStream();
        var command = new LoadFileCommand(userId, stream, extention, dto.IsAvatar);
        Result<string> loadingResult = await mediator.Send(command);

        return loadingResult.IsSuccess
            ? CreatedAtAction(nameof(GetFile), new { name = loadingResult.Value }, new { fileName = loadingResult.Value})
            : BadRequest(loadingResult.Error);
    }

    [HttpGet]
    [Route("name")]
    public async Task<IActionResult> GetFile([FromRoute] string name)
    {
        return Ok();
    }
}
