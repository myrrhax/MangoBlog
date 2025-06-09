
using Infrastructure.Utils;
using Microsoft.Extensions.Options;

namespace WebApi.Middlewares;

public class BotWhiteListRoutingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JwtConfig _jwtConfig;

    public BotWhiteListRoutingMiddleware(RequestDelegate next, IOptions<JwtConfig> jwtConfig)
    {
        _next = next;
        _jwtConfig = jwtConfig.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!(context.User.Identity?.IsAuthenticated ?? false))
        {
            await _next(context);
            return;
        }

        var endpoint = context.GetEndpoint() as RouteEndpoint;
        if (endpoint != null)
        {
            var rawPattern = endpoint.RoutePattern.RawText!;

            if (_jwtConfig.BotWhiteListUrls.Contains(rawPattern))
            {
                await _next(context);
                return;
            }
        }

        var botTokenClaim = context.User.FindFirst(_jwtConfig.BotTokenClaimName)?.Value;
        var isBotToken = string.Equals(botTokenClaim, "true", StringComparison.OrdinalIgnoreCase);
        if (isBotToken)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }
        else if (!isBotToken && endpoint != null && _jwtConfig.BotWhiteListUrls.Contains(endpoint.RoutePattern!.RawText!))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        await _next(context);
    }
}
