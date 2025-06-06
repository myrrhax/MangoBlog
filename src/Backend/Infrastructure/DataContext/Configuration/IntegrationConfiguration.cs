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

        builder.Property(integration => integration.IntegrationType)
            .IsRequired()
            .HasConversion<string>();
        builder.HasIndex(integration => integration.IntegrationType)
            .IsUnique();

        builder.HasData([
            new Integration { Id = 1, IntegrationType = IntegrationType.Telegram },
            new Integration { Id = 2, IntegrationType = IntegrationType.VKontakte },
        ]);
    }
}
