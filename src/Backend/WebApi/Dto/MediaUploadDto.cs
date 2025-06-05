using Microsoft.AspNetCore.Mvc;

namespace WebApi.Dto;

public class MediaUploadDto
{
    [FromForm(Name = "file")]
    public IFormFile? File { get; set; }

    [FromForm(Name = "isAvatar")]
    public bool IsAvatar { get; set; }
}
