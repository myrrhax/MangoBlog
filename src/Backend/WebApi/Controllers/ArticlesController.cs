using Application.Articles.Commands;
using Application.Articles.Queries;
using Application.Dto.Articles;
using Domain.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;

namespace WebApi.Controllers;

[ApiController]
[Route("api/articles")]
public class ArticlesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateArtcile([FromBody] CreateArticleRequest request)
    {
        Guid userId = User.GetUserId() ?? Guid.Empty;
        CreateArticleCommand command = new(request.Title, request.Content, userId, request.Tags);
        Result<ArticleDto> result =  await mediator.Send(command);

        return result.IsSuccess
            ? Ok(result.Value) 
            : BadRequest(result.Error);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetArticleById([FromRoute] string id)
    {
        Guid? userId = User.GetUserId();
        GetArticleByIdQuery query = new(id, userId);

        ArticleDto? dto = await mediator.Send(query);

        return dto is null
            ? NotFound()
            : Ok(dto);
    }
}
