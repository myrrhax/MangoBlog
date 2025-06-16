using System.Text;
using Application.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Domain.Utils;
using Domain.Utils.Errors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Infrastructure.Implementation;

internal class Publisher : IQueuePublisher
{
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly ILogger<Publisher> _logger;

    public Publisher(IConfiguration configuration, IConnection connection, ILogger<Publisher> logger)
    {
        _configuration = configuration;
        _connection = connection;
        _logger = logger;
    }

    public Result Publish(Publication publication)
    {
        using var channel = _connection.CreateModel();
        string exchangeName = _configuration["RabbitMq:ExchangeName"] ?? "default";
        try
        {
            channel.ExchangeDeclare(exchangeName,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                arguments: new Dictionary<string, object>());

            string jsonMessage = JsonConvert.SerializeObject(publication);
            byte[] bytes = Encoding.UTF8.GetBytes(jsonMessage);
            channel.BasicPublish(exchange: exchangeName,
                routingKey: "",
                basicProperties: null,
                body: bytes);

            _logger.LogInformation("Publication {} successfully published to queue", publication.PublicationId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred publish. Error: {}", ex.Message);

            return Result.Failure(new FailedToPublishAMessage());
        }
    }

    public Result PublishDeleteIntegration(IntegrationType type, Guid userId)
    {
        using var channel = _connection.CreateModel();
        string exchangeName = _configuration["RabbitMq:ExchangeNameIntegrations"] 
            ?? throw new ArgumentNullException(nameof(exchangeName));
        try
        {
            channel.ExchangeDeclare(exchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                arguments: new Dictionary<string, object>());
            byte[] bytes = Encoding.UTF8.GetBytes(userId.ToString());
            string deleteRoutingKey = GetDeleteRoutingKeyByType(type);

            channel.BasicPublish(exchange: exchangeName,
                routingKey: deleteRoutingKey,
                basicProperties: null,
                body: bytes);

            _logger.LogInformation("Deleting integration {} for user with id: {} is published", 
                type.ToString(),
                userId);
            return Result.Success();
        }
        catch (Exception ex) 
        {
            _logger.LogError("An error occurred publish. Error: {}", ex.Message);

            return Result.Failure(new FailedToPublishAMessage());
        }
    }

    private string GetDeleteRoutingKeyByType(IntegrationType type)
        => type switch
        {
            IntegrationType.Telegram => _configuration["RabbitMq:ExchangeNameIntegrationsKeys:DeleteTelegramIntegration"]
                ?? throw new ArgumentNullException(),
            _ => throw new ArgumentException(nameof(IntegrationType)),
        };
}
