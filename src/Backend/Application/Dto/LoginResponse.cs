namespace Application.Dto;

public record LoginResponse(string AccessToken, string RefreshToken, UserFullInfoDto User);
