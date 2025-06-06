using Application.Abstractions;
using Domain.Utils;
using Domain.Utils.Errors;
using Infrastructure.Utils;
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

    public async Task<Result> CheckGroupToken(string apiToken, string groupId)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"groups.getTokenPermissions?access_token={apiToken}&v=5.199");
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();

            var answer = JsonConvert.DeserializeObject<VkTokenPermissionsAnswer>(content);
            IEnumerable<string>? permissions = answer?.Permissions?.Select(permission => permission.Name);
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
