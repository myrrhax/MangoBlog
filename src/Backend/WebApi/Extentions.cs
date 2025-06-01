using Infrastructure.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace WebApi;

public static class Extentions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var cfg = configuration.GetSection("JwtConfig").Get<JwtConfig>();
        if (cfg is null) throw new ArgumentException("Unable to read jwt settings from appsettings.json");
        services.Configure<JwtConfig>(configuration.GetSection("JwtConfig"));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateLifetime = true,
                   ValidateAudience = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = cfg.Issuer,
                   ValidAudience = cfg.Audience,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg.SecretKey)),
               };
           });
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
            options.AddPolicy("UserOnly", policy => policy.RequireRole("user"));
        });

        return services;
    }

    public static Guid? GetUserId(this ClaimsPrincipal claims)
    {
        Claim? idClaim = claims.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier);
        return idClaim is null
            ? null
            : (Guid.TryParse(idClaim.Value, out Guid result) ? result : null);
    }
}
