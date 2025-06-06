using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataContext.Configuration;

internal class IntegrationConnectedRoomsConfiguration : IEntityTypeConfiguration<IntegrationConnectedRooms>
{
    public void Configure(EntityTypeBuilder<IntegrationConnectedRooms> builder)
    {
        builder.HasKey(entity => entity.Id);

        builder.HasOne(entity => entity.UserIntegration)
            .WithMany();

        builder.HasIndex(entity => entity.RoomId)
            .IsUnique();
    }
}
