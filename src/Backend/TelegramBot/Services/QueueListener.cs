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
using TelegramBot.Persistence;
using TelegramBot.Persistence.Entites;
using TelegramBot.Utils;

namespace TelegramBot.Services;

internal class QueueListener : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<QueueListener> _logger;
    private readonly ITelegramBotClient _bot;
    private readonly IServiceProvider _serviceProvider;
    private readonly ApiService _apiService;
    private readonly IConnection _connection;

    public QueueListener(IConfiguration configuration,
        ILogger<QueueListener> logger,
        ITelegramBotClient bot,
        IServiceProvider serviceProvider,
        ApiService apiService,
        IConnection connection)
    {
        _configuration = configuration;
        _logger = logger;
        _bot = bot;
        _serviceProvider = serviceProvider;
        _apiService = apiService;
        _connection = connection;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        RabbitMqConfiguration config = _configuration.GetSection("RabbitMq").Get<RabbitMqConfiguration>()
            ?? throw new ArgumentNullException(nameof(RabbitMqConfiguration));

        var channel = _connection.CreateModel();

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
                await PublishToChats(publication, channel, eventArgs);
            }
        };
        channel.BasicConsume(queue: config.QueueName, autoAck: false, consumer: consume);

        stoppingToken.Register(() =>
        {
            channel.Dispose();
            _logger.LogInformation("Подключение закрыто!");
        });

        return Task.CompletedTask;
    }

    private async Task PublishToChats(Publication publication, IModel channel, BasicDeliverEventArgs args)
    {
        using var scope = _serviceProvider.CreateScope();
        UsersService usersService = scope.ServiceProvider.GetRequiredService<UsersService>();
        PersistenceUser? user = await usersService.GetUserByUserId(publication.UserId);
        if (user is null)
            return;

        IntegrationPublishInfo? tgOnly = publication.IntegrationPublishInfos
            .FirstOrDefault(info => info.IntegrationType == IntegrationType.Telegram);

        if (tgOnly is null)
            return;
        IEnumerable<RoomPublishStatus> unpublishedRooms = tgOnly.PublishStatuses
            .Where(status => !status.IsPublished);

        IEnumerable<long> chatIds = unpublishedRooms.Select(status => long.Parse(status.RoomId));
        List<(string roomId, string messageId)> successfullyPublishedRoomsIds = new();

        var mediaBytes = await GetMediaAlbum(publication.MediaFiles);
        foreach (long chatId in chatIds)
        {
            try
            {
                long messageId;
                if (publication.MediaFiles.Any())
                {
                    messageId = await SendMediaAlbum(mediaBytes, publication.Content, chatId);
                }
                else
                {
                    Message msg = await _bot.SendMessage(chatId, publication.Content);
                    messageId = msg.Id;
                }
                successfullyPublishedRoomsIds.Add((chatId.ToString(), messageId.ToString()));
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to send a message to chat: {}. Reason: {}", chatId, ex.Message);
            }
        }
        IEnumerable<Task<bool>> confirmTasks = successfullyPublishedRoomsIds.Select(data => 
            _apiService.ConfirmMessageSending(publication.PublicationId, data.roomId, data.messageId, user.AccessToken));
    
        await Task.WhenAll(confirmTasks);
        channel.BasicAck(args.DeliveryTag, multiple: false);
    }

    private async Task<Dictionary<Guid, (MediaFileType Type, byte[] Bytes)>> GetMediaAlbum(IEnumerable<(Guid Id, MediaFileType Type)> medias)
    {
        var result = new Dictionary<Guid, (MediaFileType, byte[])>();

        foreach (var mediasItem in medias)
        {
            byte[]? bytes = await _apiService.GetMediaFile(mediasItem.Id);
            if (bytes is null)
                continue;
            result[mediasItem.Id] = (mediasItem.Type, bytes);
        }

        return result;
    }

    private async Task<long> SendMediaAlbum(Dictionary<Guid, (MediaFileType Type, byte[] Bytes)> medias, string caption, long chatId)
    {
        var album = new List<IAlbumInputMedia>();
        foreach (Guid id in medias.Keys)
        {
            var mediasItem = medias[id];
            var ms = new MemoryStream(mediasItem.Bytes);
            InputMedia media = mediasItem.Type == MediaFileType.Photo
                ? new InputMediaPhoto(ms)
                : new InputMediaVideo(ms);
            if (!album.Any())
            {
                media.Caption = caption;
            }
            album.Add((IAlbumInputMedia)media);
        }

        Message[] messages = await _bot.SendMediaGroup(chatId, album);
        
        return messages.First().Id;
    }
}
