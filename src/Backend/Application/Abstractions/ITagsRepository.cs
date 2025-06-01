using Domain.Entities;
using Domain.Utils;

namespace Application.Abstractions;

public interface ITagsRepository
{
    Task<Result> AddTag(Tag tag, CancellationToken cancellationToken);
    Task<IEnumerable<Tag>> AddTagsIfAbsent(IEnumerable<string> names, CancellationToken cancellationToken);
    Task<Tag?> GetTagByName(string name, CancellationToken cancellationToken);
    Task<Tag?> GetTagById(int id, CancellationToken cancellationToken);
    Task<Result<IEnumerable<Tag>>> GetTagsWithNames(IEnumerable<string> tagNames, CancellationToken cancellationToken);
}