namespace Application.Dto;

public record RegistrationResponse(string AccessToken, string RefreshToken, UserFullInfoDto User);
