using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TelegramBot.Utils;

namespace TelegramBot.Services;

internal class PublicationsListener : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PublicationsListener> _logger;

    public PublicationsListener(IConfiguration configuration, ILogger<PublicationsListener> logger)
    {
        _configuration = configuration;
        _logger = logger;
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

        channel.ExchangeDeclare(config.ExchangeName,
            type: ExchangeType.Fanout,
            durable: true,
            autoDelete: false,
            arguments: new Dictionary<string, object>());

        channel.QueueDeclare(queue: config.QueueName,
            durable: true,
            autoDelete: false,
            arguments: new Dictionary<string, object>());

        channel.QueueBind(queue: config.QueueName,
            exchange: config.ExchangeName,
            routingKey: config.RoutingKey);

        var consume = new EventingBasicConsumer(channel);

        consume.Received += (sender, eventArgs) =>
        {
            byte[] body = eventArgs.Body.ToArray();
            string message = Encoding.UTF8.GetString(body);

            _logger.LogInformation("Новое сообщение: {}", message);
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
}
