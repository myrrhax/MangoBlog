namespace WebApi.Dto;

public record AddPublicationRequestDto(string Content,
    IEnumerable<Guid> MediaIds,
    DateTime? PublicationDate = null);
