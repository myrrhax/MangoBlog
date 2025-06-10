namespace TelegramBot.Utils;

internal class RabbitMqConfiguration
{
    public string Host { get; set; } = string.Empty;
    public string ExchangeName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Pass { get; set; } = string.Empty;
    public string QueueName { get; set; } = string.Empty;
    public string RoutingKey { get; set; } = string.Empty;
}
