using Application.MediFiles.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/media")]
public class MediaController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> LoadFile([FromForm] IFormFile file, [FromForm] bool isAvatar)
    {
        Guid userId = User.GetUserId()!.Value;
        string extention = Path.GetExtension(file.FileName).ToLower();
        string? actionUrl = Url.Action(action: nameof(GetFile),
            controller: "Media",
            values: new { },
            protocol: Request.Scheme);

        using Stream stream = file.OpenReadStream();
        var command = new LoadFileCommand(userId, stream, extention, actionUrl ?? string.Empty, isAvatar);
    }

    [HttpGet]
    [Route("name")]
    public async Task<IActionResult> GetFile([FromRoute] string name)
    {
        return Ok();
    }
}
