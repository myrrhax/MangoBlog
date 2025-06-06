using System.Net.Http.Json;
using Application.Abstractions;
using Domain.Utils;
using Domain.Utils.Errors;
using Infrastructure.Utils.VkAnswers;
using Newtonsoft.Json;

namespace Infrastructure.Implementation;

internal class VkApiServiceImpl : IVkApiService
{
    private const string PermissionNameForPublications = "wall";
    private readonly HttpClient _httpClient;

    public VkApiServiceImpl(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result> CheckTokenPermissions(string apiToken, string groupId)
    {
        try
        {
            var jsonObject = new { accessToken = apiToken };
            string json = JsonConvert.SerializeObject(jsonObject);
            HttpResponseMessage response = await _httpClient.GetAsync($"groups.getTokenPermissions?v=5.199&access_token={apiToken}");
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();

            var answer = JsonConvert.DeserializeObject<VkAnswerResponse>(content);
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
}
