namespace Application.Dto;

public class UserFullInfoDto : UserDto
{
    public required string Email { get; set; }
    public required string Login { get; set; }
    public DateTime RegistrationTime { get; set; }
}