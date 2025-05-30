namespace Application.Dto;

public record RefreshResponse(string AccessToken, string RefreshToken, UserFullInfoDto User);
