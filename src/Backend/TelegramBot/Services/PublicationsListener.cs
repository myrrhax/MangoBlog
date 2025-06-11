using System.Text;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Api;
using TelegramBot.Utils;

namespace TelegramBot.Services;

internal class PublicationsListener : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PublicationsListener> _logger;
    private readonly ITelegramBotClient _bot;
    private readonly IServiceProvider _serviceProvider;
    private readonly ApiService _apiService;

    public PublicationsListener(IConfiguration configuration,
        ILogger<PublicationsListener> logger,
        ITelegramBotClient bot,
        IServiceProvider serviceProvider,
        ApiService apiService)
    {
        _configuration = configuration;
        _logger = logger;
        _bot = bot;
        _serviceProvider = serviceProvider;
        _apiService = apiService;
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

        consume.Received += async (sender, eventArgs) =>
        {
            byte[] body = eventArgs.Body.ToArray();
            string json = Encoding.UTF8.GetString(body);
            var publication = JsonConvert.DeserializeObject<Publication>(json);
            if (publication is not null)
            {
                _logger.LogInformation("Новая публикация: {}", publication.PublicationId);
                await PublishToChats(publication);
            }
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

    private async Task PublishToChats(Publication publication)
    {
        IntegrationPublishInfo? tgOnly = publication.IntegrationPublishInfos
            .FirstOrDefault(info => info.IntegrationType == Domain.Enums.IntegrationType.Telegram);

        if (tgOnly is null)
            return;
        IEnumerable<RoomPublishStatus> unpublishedRooms = tgOnly.PublishStatuses
            .Where(status => !status.IsPublished);

        IEnumerable<long> chatIds = unpublishedRooms.Select(status => long.Parse(status.RoomId));
        List<string> successfullyPublishedRoomsIds = new();
        foreach (long chatId in chatIds)
        {
            try
            {
                if (publication.MediaFiles.Any())
                {
                    List<IAlbumInputMedia> album = await GetMediaAlbum(publication.MediaFiles, publication.Content);

                    await _bot.SendMediaGroup(chatId, album);
                }
                else
                {
                    await _bot.SendMessage(chatId, publication.Content);
                }
                successfullyPublishedRoomsIds.Add(chatId.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to send a message to chat: {}. Reason: {}", chatId, ex.Message);
            }
        }
    }

    private async Task<List<IAlbumInputMedia>> GetMediaAlbum(IEnumerable<(Guid Id, MediaFileType Type)> medias, string caption)
    {
        var result = new List<IAlbumInputMedia>();

        foreach (var mediasItem in medias)
        {
            using Stream? fs = await _apiService.GetMediaFile(mediasItem.Id);
            if (fs is null)
                continue;

            InputMedia media = mediasItem.Type == MediaFileType.Photo
                ? new InputMediaPhoto(fs)
                : new InputMediaVideo(fs);

            if (!result.Any())
            {
                media.Caption = caption;
            }
            result.Add((IAlbumInputMedia) media);
        }

        return result;
    }
}
