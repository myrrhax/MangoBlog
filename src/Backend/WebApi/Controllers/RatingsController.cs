using Application.Ratings.Commands;
using Domain.Utils.Errors;
using Domain.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;
using Application.Ratings.Queries;
using Application.Dto.Articles;

namespace WebApi.Controllers;

[ApiController]
[Route("api/ratings")]
[Authorize]
public class RatingsController(IMediator mediator) : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> AddRating([FromBody] AddRatingRequest request)
    {
        var command = new AddRatingToPostCommand(request.PostId, User.GetUserId()!.Value, request.RatingType);
        Result result = await mediator.Send(command);

        return result.IsSuccess
            ? Ok()
            : result.Error switch
            {
                ArticleNotFound => NotFound(),
                _ => BadRequest(result.Error)
            };
    }

    [HttpGet]
    [Route("my")]
    public async Task<IActionResult> GetUserRatedPosts()
    {
        Guid userId = User.GetUserId()!.Value;
        var command = new GetMyRatedPostsCommand(userId);
        IEnumerable<ArticleDto> articles = await mediator.Send(command);

        return articles.Any()
            ? Ok(articles)
            : NotFound();
    }
}
