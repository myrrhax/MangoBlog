namespace Application.Dto;

public record RoomPublishStatusDto(string RoomId,
    string MessageId,
    string ChannelName,
    bool IsPublished);
