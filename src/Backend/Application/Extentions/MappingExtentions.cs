using Application.Dto;
using Domain.Entities;

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
}
