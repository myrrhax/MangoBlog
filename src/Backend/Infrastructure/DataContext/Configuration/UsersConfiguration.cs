using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataContext.Configuration;

internal class UsersConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Login)
            .HasMaxLength(25)
            .IsRequired();
        builder.HasIndex(entity => entity.Login)
            .IsUnique();

        builder.Property(entity => entity.Email)
            .IsRequired();
        builder.HasIndex(entity => entity.Email)
            .IsUnique();

        builder.Property(entity => entity.DisplayedName)
            .IsRequired();

        builder.Property(entity => entity.FirstName)
            .HasMaxLength(25)
            .IsRequired();

        builder.Property(entity => entity.LastName)
            .HasMaxLength(25)
            .IsRequired();

        builder.HasMany(user => user.RefreshTokens)
            .WithOne(token => token.User)
            .HasForeignKey(token => token.UserId);

        builder.HasMany(user => user.Subscriptions)
            .WithMany();

        builder.Property(user => user.Role)
            .IsRequired()
            .HasDefaultValue(Role.User)
            .HasConversion<string>();
    }
}
