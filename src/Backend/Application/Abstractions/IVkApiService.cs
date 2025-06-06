using Domain.Utils;

namespace Application.Abstractions;

public interface IVkApiService
{
    Task<Result> CheckTokenPermissions(string apiToken);
    Task<Result<(string groupId, string groupName)>> GetTokenGroupId(string apiToken);
}
