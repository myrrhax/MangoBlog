﻿namespace TelegramBot.Persistence.Entites;

public class User
{
    public long TelegramId { get; set; }
    public Guid UserId { get; set; }
    public string AccessToken { get; set; } = string.Empty;
}
