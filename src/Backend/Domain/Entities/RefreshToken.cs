namespace Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string Token { get; set; }
    public DateTime ExpirationDate { get; set; }

    public ApplicationUser User { get; set; } = null!;
}
