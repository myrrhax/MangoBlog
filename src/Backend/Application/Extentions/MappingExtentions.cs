using Application.Dto;
using Application.Dto.Articles;
using Application.Dto.Integrations;
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
            RegistrationTime = entity.RegistrationTime.ToLocalTime(),
            Integration = entity.Integration?.MapToDto(),
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
            entity.CreationDate.ToLocalTime(), 
            entity.Likes, 
            entity.Dislikes,
            reaction?.ToString(),
            entity.CoverImageId);
    }

    public static IntegrationDto MapToDto(this Integration entity)
        => new IntegrationDto(entity.User.MapToDto(), entity.TelegramIntegration?.MapToDto());

    public static TelegramIntegrationDto MapToDto(this TelegramIntegration entity)
        => new TelegramIntegrationDto(entity.IntegrationCode,
                entity.TelegramId, 
                entity.IsConfirmed,
                entity.ConnectedChannels.Select(channel => channel.MapToDto()));

    public static TelegramChannelDto MapToDto(this TelegramChannel entity)
        => new TelegramChannelDto(entity.Name, entity.ChannelId);

    public static PublicationDto MapToDto(this Publication entity, Dictionary<string, string>? channelNames = null)
        => new PublicationDto(entity.PublicationId,
            entity.UserId,
            entity.Content,
            entity.MediaFiles.Select(file => new MediaFileDto(file.Id, file.Type.ToString())),
            entity.CreationDate.ToLocalTime(),
            entity.PublicationTime,
            entity.IntegrationPublishInfos.Select(info => info.MapToDto(channelNames)));

    public static IntegrationPublishInfoDto MapToDto(this IntegrationPublishInfo entity, Dictionary<string, string>? channelNames = null)
        => new IntegrationPublishInfoDto(entity.IntegrationType.ToString(),
            entity.PublishStatuses.Select(status => status.MapToDto(channelNames?[status.RoomId])));

    public static RoomPublishStatusDto MapToDto(this RoomPublishStatus entity, string? channelName = null)
        => new RoomPublishStatusDto(entity.RoomId,
            entity.MessageId,
            channelName,
            entity.IsPublished);
}
