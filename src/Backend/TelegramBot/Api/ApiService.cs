using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TelegramBot.Dto;

namespace TelegramBot.Api;

internal class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiService> _logger;

    public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
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
