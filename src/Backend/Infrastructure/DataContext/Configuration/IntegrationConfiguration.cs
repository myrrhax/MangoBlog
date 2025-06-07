using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataContext.Configuration;

internal class IntegrationConfiguration : IEntityTypeConfiguration<Integration>
{
    public void Configure(EntityTypeBuilder<Integration> builder)
    {
        builder.HasKey(integration => integration.Id);

        builder.HasOne(entity => entity.User)
            .WithMany(user => user.Integrations)
            .HasForeignKey(entity => entity.UserId);

        builder.HasOne(entity => entity.TelegramIntegration)
            .WithOne(tgIntegration => tgIntegration.Integration)
            .HasForeignKey<Integration>(entity => entity.TelegramIntegrationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(entity => entity.User)
            .WithMany(user => user.Integrations)
            .HasForeignKey(entity => entity.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
