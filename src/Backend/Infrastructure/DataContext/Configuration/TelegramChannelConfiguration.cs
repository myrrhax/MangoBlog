using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataContext.Configuration;

internal class TelegramChannelConfiguration : IEntityTypeConfiguration<TelegramChannel>
{
    public void Configure(EntityTypeBuilder<TelegramChannel> builder)
    {
        builder.HasKey(entity => entity.Id);

        builder.HasOne(entity => entity.TelegramIntegration)
            .WithMany(tgIntegration => tgIntegration.ConnectedChannels)
            .HasForeignKey(entity => entity.TelegramIntegrationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(entity => entity.Name)
            .IsRequired();

        builder.Property(entity => entity.ChannelId)
            .IsRequired();
        builder.HasIndex(entity => entity.ChannelId)
            .IsUnique();
    }
}
