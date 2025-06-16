using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Telegram.Bot;
using TelegramBot.Api;
using TelegramBot.Utils;
using System.Text;
using TelegramBot.Persistence.Entites;
using Microsoft.Extensions.DependencyInjection;
using TelegramBot.Persistence;

namespace TelegramBot.Services;

internal class DeleteTelegramIntegrationListener : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PublicationsListener> _logger;
    private readonly ITelegramBotClient _bot;
    private readonly IServiceProvider _serviceProvider;

    public DeleteTelegramIntegrationListener(IConfiguration configuration,
        ILogger<PublicationsListener> logger,
        ITelegramBotClient bot,
        IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _logger = logger;
        _bot = bot;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        RabbitMqConfiguration config = _configuration.GetSection("RabbitMq").Get<RabbitMqConfiguration>()
            ?? throw new ArgumentNullException(nameof(RabbitMqConfiguration));

        var factory = new ConnectionFactory
        {
            HostName = config.Host,
            UserName = config.Name,
            Password = config.Pass,
        };

        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.ExchangeDeclare(config.ExchangeNameIntegrations,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: new Dictionary<string, object>());

        string queueName = config.ExchangeNameIntegrationsKeys["DeleteTelegramIntegrationQueue"];
        channel.QueueDeclare(queue: queueName,
            durable: true,
            autoDelete: false,
            arguments: new Dictionary<string, object>());

        channel.QueueBind(queue: config.QueueName,
            exchange: config.ExchangeName,
            routingKey: config.ExchangeNameIntegrationsKeys["DeleteTelegramIntegrationKey"]);

        var consume = new EventingBasicConsumer(channel);

        consume.Received += async (sender, eventArgs) =>
        {
            byte[] body = eventArgs.Body.ToArray();
            string idString = Encoding.UTF8.GetString(body);
            if (!Guid.TryParse(idString, out Guid id))
            {
                return;
            }

            _logger.LogInformation("New id for remove: {}", id);
            await RemoveUser(id);
        };
        channel.BasicConsume(queue: config.QueueName, autoAck: true, consumer: consume);

        stoppingToken.Register(() =>
        {
            connection.Dispose();
            channel.Dispose();
            _logger.LogInformation("Подключение закрыто!");
        });

        return Task.CompletedTask;
    }

    private async Task RemoveUser(Guid id)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<UsersService>();

        PersistenceUser? user = await service.GetUserByUserId(id);
        if (user is null)
            return;
        bool result = await service.DeleteUserById(id);

        if (result)
        {
            await _bot.SendMessage(user.TelegramId, "Ваша интеграция была удалена");
        }
    }
}
