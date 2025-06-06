using Domain.Utils;

namespace Application.Abstractions;

public interface IVkApiService
{
    Task<Result> CheckGroupToken(string apiToken, string groupId);
}
