using Application.Abstractions;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Infrastructure.Implementation;

internal class Publisher : IPublisher
{
    private readonly IConfiguration _configuration;

    public Publisher(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void PublishMessage(string id, string message)
    {
        string host = _configuration["RabbitMq:Host"] ?? "localhost";
        string exhange = _configuration["RabbitMq:ExchangeName"] ?? "default";

        var factory = new ConnectionFactory()
        {
            HostName = host
        };
    }
}
