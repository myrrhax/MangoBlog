using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using TelegramBot.Persistence.Entites;

namespace TelegramBot.Persistence;

internal class UsersService
{
    private readonly IDbConnection _dbConnection;
    private readonly ILogger<UsersService> _logger;

    public UsersService(IDbConnection dbConnection, ILogger<UsersService> logger)
    {
        _dbConnection = dbConnection;
        _logger = logger;
    }

    public async Task<bool> AddUser(PersistenceUser user)
    {
        const string sql = """
            INSERT INTO users (telegram_id, user_id, api_token)
            VALUES (@TelegramId, @UserId, @AccessToken);
        """;
        try
        {
            int rows = await _dbConnection.ExecuteAsync(sql, new { user.TelegramId, user.UserId, user.AccessToken });

            return rows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to add new user (tgId: {}) to database: {}", user.TelegramId, ex.Message);
            return false;
        }
    }

    public async Task InitializeTable()
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS Users (
                telegram_id BIGINT PRIMARY KEY,
                user_id UUID NOT NULL UNIQUE,
                api_token TEXT NOT NULL UNIQUE
            );
        """;

        await _dbConnection.ExecuteAsync(sql);
    }
}
