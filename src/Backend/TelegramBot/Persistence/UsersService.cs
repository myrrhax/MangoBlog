using System.Data;
using Dapper;

namespace TelegramBot.Persistence;

internal class UsersService
{
    private readonly IDbConnection _dbConnection;

    public UsersService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task InitializeTable()
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS Users (
                telegram_id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
                user_id UUID NOT NULL UNIQUE,
                api_token VARCHAR(255) NOT NULL UNIQUE
            );
        """;

        await _dbConnection.ExecuteAsync(sql);
    }
}
