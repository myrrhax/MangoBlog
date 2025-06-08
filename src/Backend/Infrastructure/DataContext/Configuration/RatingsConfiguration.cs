using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataContext.Configuration;

public class RatingsConfiguration : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.HasKey(entity => new { entity.UserId, entity.ArticleId });

        builder.Property(entity => entity.RatingType)
            .IsRequired()
            .HasConversion<string>();

        builder.HasOne(rating => rating.User)
            .WithMany()
            .HasForeignKey(rating => rating.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
