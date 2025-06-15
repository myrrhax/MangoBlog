using Application.Articles.Commands;
using Application.Articles.Queries;
using Application.Dto.Articles;
using Domain.Utils;
using Domain.Utils.Errors;
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
        CreateArticleCommand command = new(request.Title, request.Content, userId, request.Tags, request.CoverImageId);
        Result<ArticleDto> result =  await mediator.Send(command);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetArticleById), new { id = result.Value!.Id }, result.Value) 
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
            : Ok(new { data = dto });
    }

    [HttpGet]
    public async Task<IActionResult> GetArticles([FromQuery] IEnumerable<string>? tags = null,
        [FromQuery] string? query = null,
        [FromQuery] string? sortByDate = null,
        [FromQuery] string? sortByPopularity = null,
        [FromQuery] Guid? authorId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var command = new GetArticlesQuery(tags, query, sortByDate, sortByPopularity, authorId, page, pageSize);
        Result<(IEnumerable<ArticleDto>, int)> result = await mediator.Send(command);

        if (result.IsSuccess)
        {
            (var articles, int count) = result.Value!;

            return articles.Any()
                ? Ok(new { articles, totalCount = count })
                : NotFound();
        }

        return BadRequest(result.Error);
    }

    [HttpGet]
    [Route("my")]
    [Authorize]
    public async Task<IActionResult> GetMyArticles()
    {
        Guid userId = User.GetUserId()!.Value;
        GetMyArticlesQuery query = new(userId);

        IEnumerable<ArticleDto> dtos = await mediator.Send(query);

        return dtos.Any()
            ? Ok(new { data = dtos })
            : NotFound();
    }

    [HttpDelete]
    [Route("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteArticleById([FromRoute] string id)
    {
        Guid userId = User.GetUserId()!.Value;
        DeleteArticleCommand command = new(id, userId);
        Result commandResult = await mediator.Send(command);

        return commandResult.IsSuccess
            ? Ok()
            : commandResult.Error switch 
            {
                UserNotFound or NoPermission => Forbid(),
                ArticleNotFound => NotFound(),
                _ => BadRequest()
            };
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateArticle([FromBody] UpdateArticleRequest request)
    {
        Guid userId = User.GetUserId()!.Value;
        var command = new UpdateArticleCommand(request.Id, userId, request.Title, request.Content, request.Tags, request.CoverImageId);
        Result<ArticleDto> updateResult = await mediator.Send(command);

        return updateResult.IsSuccess
            ? Ok(updateResult.Value)
            : updateResult.Error switch
            {
                UserNotFound or NoPermission => Forbid(),
                ArticleNotFound => NotFound(),
                ApplicationValidationError => BadRequest(updateResult.Error),
                _ => BadRequest()
            };
    }
}
