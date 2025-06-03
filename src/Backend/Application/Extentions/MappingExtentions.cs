using Application.Dto;
using Application.Dto.Articles;
using Domain.Entities;
using Domain.Enums;

namespace Application.Extentions;

internal static class MappingExtentions
{
    public static UserDto MapToDto(this ApplicationUser entity)
    {
        return new UserDto()
        {
            Id = entity.Id,
            DisplayedName = entity.DisplayedName,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            AvatarUrl = entity.AvatarUrl,
            BirthDate = entity.BirthDate,
            Role = entity.Role.ToString()
        };
    }

    public static UserFullInfoDto MapToFullInfo(this ApplicationUser entity)
    {
        return new UserFullInfoDto()
        {
            Id = entity.Id,
            DisplayedName = entity.DisplayedName,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            AvatarUrl = entity.AvatarUrl,
            BirthDate = entity.BirthDate,
            Email = entity.Email,
            Login = entity.Login,
            RegistrationTime = entity.RegistrationTime,
            Subscriptions = entity.Subscriptions.Select(sub => sub.MapToDto())
        };
    }

    public static ArticleDto MapToDto(this Article entity, ApplicationUser? creator, int likes, int dislikes, RatingType? reaction = null)
    {
        return new ArticleDto(entity.Id, 
            creator?.MapToDto(),
            entity.Title,
            entity.Content, 
            entity.Tags, 
            entity.CreationDate, 
            likes, 
            dislikes,
            reaction);
    }
}
