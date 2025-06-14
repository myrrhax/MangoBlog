using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using TelegramBot.Dto;
using TelegramBot.Persistence.Entites;

namespace TelegramBot.Api;

internal class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiService> _logger;

    public ApiService(HttpClient httpClient, ILogger<ApiService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> AttachTelegramChannel(PersistenceUser user, string channelId, string channelName)
    {
        var body = new { ChatId = channelId,  ChatName = channelName };
        string json = JsonConvert.SerializeObject(body);
        var bodyContent = new StringContent(json, encoding: Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, "api/integrations/tg/add-channel");
        request.Content = bodyContent;
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user.AccessToken);

        try
        {
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to add channel for user: {}. Error: {}", user.UserId, ex.Message);
            return false;
        }
    }

    public async Task<bool> ConfirmMessageSending(string publicationId, string roomId, string messageId, string apiToken)
    {
        var body = new { publicationId, roomId, messageId, integrationType = "tg" };
        string json = JsonConvert.SerializeObject(body);
        var content = new StringContent(json);
        var request = new HttpRequestMessage(HttpMethod.Post, "api/publications/confirm");
        request.Content = content;
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);

        try
        {
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return true;
        }
        catch(Exception)
        {
            return false;
        }
    }

    public async Task<byte[]?> GetMediaFile(Guid fileId)
    {
        try
        {
            var response = await _httpClient.GetAsync("api/media/" + fileId);
            response.EnsureSuccessStatusCode();
            byte[] bytes = await response.Content.ReadAsByteArrayAsync();
            return bytes;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<ConfirmationResponseDto?> ConfirmTelegramIntegration(string integrationCode, string telegramId)
    {
        var body = new { telegramId, integrationCode };
        string json = JsonConvert.SerializeObject(body);
        var bodyContent = new StringContent(json, encoding: Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync("api/integrations/tg/confirm", bodyContent);
            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            var responseDto = JsonConvert.DeserializeObject<ConfirmationResponseDto>(jsonResponse);

            return responseDto;
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Unable to add integration for tg user: {}. Error: {}", telegramId, ex.Message);
            return null;
        }
    }
}
