using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataContext.Configuration;

internal class TelegramIntegrationConfiguration : IEntityTypeConfiguration<TelegramIntegration>
{
    public void Configure(EntityTypeBuilder<TelegramIntegration> builder)
    {
        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.IntegrationCode)
            .IsRequired();

        builder.HasIndex(entity => entity.IntegrationCode)
            .IsUnique();

        builder.HasOne(entity => entity.Integration)
            .WithOne(entity => entity.TelegramIntegration)
            .HasForeignKey<TelegramIntegration>(tgIntegration => tgIntegration.IntegrationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(entity => entity.ConnectedChannels)
            .WithOne(channel => channel.TelegramIntegration)
            .HasForeignKey(channel => channel.TelegramIntegrationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
