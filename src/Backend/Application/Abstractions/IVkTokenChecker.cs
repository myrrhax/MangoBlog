using Domain.Utils;

namespace Application.Abstractions;

public interface IVkTokenChecker
{
    Task<Result> CheckGroupToken(string apiToken, string groupId);
}
