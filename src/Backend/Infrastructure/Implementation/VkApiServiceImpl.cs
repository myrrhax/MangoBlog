using System.Net.Http.Json;
using Application.Abstractions;
using Domain.Utils;
using Domain.Utils.Errors;
using Infrastructure.Utils.VkAnswers.GetById;
using Infrastructure.Utils.VkAnswers.TokenValid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Infrastructure.Implementation;

internal class VkApiServiceImpl : IVkApiService
{
    private const string PermissionNameForPublications = "wall";
    private readonly HttpClient _httpClient;
    private readonly ILogger<VkApiServiceImpl> _logger;

    public VkApiServiceImpl(HttpClient httpClient, ILogger<VkApiServiceImpl> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Result> CheckTokenPermissions(string apiToken)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"groups.getTokenPermissions?v=5.199&access_token={apiToken}");
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();

            var answer = JsonConvert.DeserializeObject<VkTokenValidAnswerResponse>(content);
            IEnumerable<string>? permissions = answer?.Response?.Permissions?.Select(permission => permission.Name);
            if (permissions is null || !permissions!.Contains(PermissionNameForPublications))
            {
                return Result.Failure(new ApiTokenHasNoPermission());
            }

            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure(new InvalidApiToken());
        }
    }

    public async Task<Result<(string, string)>> GetTokenGroupId(string apiToken)
    {
        try
        {
            var response = await _httpClient.GetAsync($"groups.getById?v=5.199&access_token={apiToken}");
            response.EnsureSuccessStatusCode();
            string jsonString = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<GetGroupByIdResponse>(jsonString);
            IEnumerable<VkGroup>? groups = deserializedResponse?.Response?.Groups;

            if (groups is null || !groups.Any())
                return Result.Failure<(string, string)>(new VkGroupNotFound());

            VkGroup group = groups.First();
            _logger.LogInformation("New integration for group: {}", group.Name);

            return Result.Success((group.Id.ToString(), group.Name));
        }
        catch (Exception ex)
        {
            return Result.Failure<(string, string)>(new InvalidApiToken());
        }
    }
}
