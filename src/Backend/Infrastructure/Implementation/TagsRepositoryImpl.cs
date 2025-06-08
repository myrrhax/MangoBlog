using Application.Abstractions;
using Domain.Entities;
using Domain.Utils;
using Domain.Utils.Errors;
using Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Implementation;

internal class TagsRepositoryImpl(ApplicationDbContext context, ILogger<TagsRepositoryImpl> logger) : ITagsRepository
{
    public Task<Result> AddTag(Tag tag, CancellationToken cancellationToken)
    {
        throw new NotImplementedException(); // ToDo implement later
    }

    public async Task<Result<IEnumerable<Tag>>> AddTagsIfAbsent(IEnumerable<string> names, CancellationToken cancellationToken)
    {
        try
        {
            List<string> tagNames = await context.Tags
                .Where(tag => names.Contains(tag.Name))
                .Select(tag => tag.Name)
                .ToListAsync(cancellationToken);
            HashSet<string> containedTags = tagNames.ToHashSet();
            
            List<Tag> absentTags = names
                .Where(name => !containedTags.Contains(name))
                .Select(tag => new Tag { Name = tag })
                .ToList();

            if (absentTags.Any())
            {
                await context.Tags.AddRangeAsync(absentTags, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
            }

            logger.LogInformation("New tags were inserted: [{}]", string.Join(", ", absentTags.Select(tag => tag.Name)));
            var tags = await context.Tags
                .Where(t => names.Contains(t.Name))
                .ToListAsync(cancellationToken);

            return Result.Success(tags.AsEnumerable());
        }
        catch (Exception ex)
        {
            logger.LogError("An error occurred tag insertion: {}\nStack trace: {}", ex.Message, ex.StackTrace);

            return Result.Failure<IEnumerable<Tag>>(new DatabaseInteractionError("Failed to insert tags"));
        }
    }

    public Task<Tag?> GetTagById(int id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException(); // ToDo implement later
    }

    public Task<Tag?> GetTagByName(string name, CancellationToken cancellationToken)
    {
        throw new NotImplementedException(); // ToDo implement later
    }

    public Task<Result<IEnumerable<Tag>>> GetTagsWithNames(IEnumerable<string> tagNames, CancellationToken cancellationToken)
    {
        throw new NotImplementedException(); // ToDo implement later
    }
}
