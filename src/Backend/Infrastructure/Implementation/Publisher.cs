using System.Text;
using Application.Abstractions;
using Domain.Entities;
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
}
