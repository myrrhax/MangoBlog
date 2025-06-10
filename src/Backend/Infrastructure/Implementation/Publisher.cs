using System.Text;
using Application.Abstractions;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Infrastructure.Implementation;

internal class Publisher : IPublisher
{
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;

    public Publisher(IConfiguration configuration, IConnection connection)
    {
        _configuration = configuration;
        _connection = connection;
    }

    public void PublishMessage(string id, string message)
    {
        using var channel = _connection.CreateModel();
        string exchangeName = _configuration["RabbitMq:ExchangeName"] ?? "default";

        channel.ExchangeDeclare(exchangeName,
            type: ExchangeType.Fanout,
            durable: true,
            autoDelete: false,
            arguments: new Dictionary<string, object>());

        byte[] bytes = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: exchangeName, 
            routingKey: "",
            basicProperties: null,
            body: bytes);
    }
}
