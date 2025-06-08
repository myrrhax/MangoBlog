using Domain.Enums;

namespace Domain.Entities;

public class Rating
{
    public Guid UserId { get; set; }
    public required string ArticleId { get; set; }
    public RatingType RatingType { get; set; }
    public DateTime CreationDate { get; set; }

    public ApplicationUser User { get; set; } = null!;
}
