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
            AvatarId = entity.Avatar?.Id.ToString(),
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
            AvatarId = entity.Avatar?.Id.ToString(),
            BirthDate = entity.BirthDate,
            Email = entity.Email,
            Login = entity.Login,
            Role = entity.Role.ToString(),
            Integrations = entity.Integrations.Select(integration => integration.MapToDto()),
            RegistrationTime = entity.RegistrationTime,
            Subscriptions = entity.Subscriptions.Select(sub => sub.MapToDto())
        };
    }

    public static ArticleDto MapToDto(this Article entity, ApplicationUser? creator, RatingType? reaction = null)
    {
        return new ArticleDto(entity.Id, 
            creator?.MapToDto(),
            entity.Title,
            entity.Content, 
            entity.Tags, 
            entity.CreationDate, 
            entity.Likes, 
            entity.Dislikes,
            reaction?.ToString());
    }

    public static IntegrationDto MapToDto(this UserIntegration entity)
        => new IntegrationDto
        {
            AccountId = entity.AccountId,
            IntegrationType = entity.Integration.IntegrationType.ToString(),
            IsConfirmed = entity.IsConfirmed,
            RoomId = entity.RoomId,
        };
}
