
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
    private readonly IUserRepository _userRepository;

    public ExpiredRefreshTokenDeleteService(IOptions<JwtConfig> jwtConfig,
        ILogger<ExpiredRefreshTokenDeleteService> logger,
        IUserRepository userRepository)
    {
        _jwtConfig = jwtConfig;
        _logger = logger;
        _userRepository = userRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        TimeSpan delay = TimeSpan.FromHours(_jwtConfig.Value.RefreshTokenDeleteTimeoutInHours);
        while (!stoppingToken.IsCancellationRequested)
        {
            Result<int> deleteResult = await _userRepository.DeleteExpiredTokens(stoppingToken);
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
