
using Application.Abstractions;
using Domain.Utils;
using Domain.Utils.Errors;
using Infrastructure.Utils;
using Microsoft.Extensions.Options;

namespace WebApi.BackgroundServices;

public class ExpiredRefreshTokenDeleteService : BackgroundService
{
    private readonly IOptions<JwtConfig> _jwtConfig;
    private readonly ILogger<ExpiredRefreshTokenDeleteService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public ExpiredRefreshTokenDeleteService(IOptions<JwtConfig> jwtConfig,
        ILogger<ExpiredRefreshTokenDeleteService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _jwtConfig = jwtConfig;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{} - Starting service{}...",
            DateTime.Now,
            nameof(ExpiredRefreshTokenDeleteService));
        TimeSpan delay = TimeSpan.FromHours(_jwtConfig.Value.RefreshTokenDeleteTimeoutInHours);
        while (!stoppingToken.IsCancellationRequested)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            IUserRepository userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            Result<int> deleteResult = await userRepository.DeleteExpiredTokens(stoppingToken);
            if (deleteResult.IsFailure && deleteResult.Error is DatabaseInteractionError error)
            {
                _logger.LogError("{} - Failed to connect to database. Error: {}", 
                    DateTime.Now,
                    error.Message);
            }
            else
            {
                _logger.LogInformation("{} - Service deleted: {} expired tokens",
                    DateTime.Now,
                    deleteResult.Value);
            }
            await Task.Delay(delay);
        }
    }
}
