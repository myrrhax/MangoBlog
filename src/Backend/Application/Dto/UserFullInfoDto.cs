using Application.Dto.Integrations;
using Domain.Entities;

namespace Application.Dto;

public class UserFullInfoDto : UserDto
{
    public string Email { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public DateTime RegistrationTime { get; set; }
    public IEnumerable<UserDto> Subscriptions { get; set; } = [];
    public IntegrationDto? Integration { get; set; }
}