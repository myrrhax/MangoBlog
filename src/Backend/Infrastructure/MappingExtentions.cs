using Domain.Entities;
using Infrastructure.MongoModels;

namespace Infrastructure;

internal static class MappingExtentions
{
    public static Article MapToEntity(this ArticleDocument document)
    {
        return new Article(document.Id,
            document.Title,
            document.Content.ToDictionary(),
            document.CreatorId,
            document.Tags,
            document.CreationDate);
    }
}
