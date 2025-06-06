using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataContext.Configuration;

internal class UserIntegrationsConfiguration : IEntityTypeConfiguration<UserIntegration>
{
    public void Configure(EntityTypeBuilder<UserIntegration> builder)
    {
        builder.HasKey(userIntegration => new { userIntegration.UserId, userIntegration.IntegrationId });

        builder.HasOne(userIntegration => userIntegration.User)
            .WithMany(userIntegration => userIntegration.Integrations)
            .HasForeignKey(userIntegration => userIntegration.IntegrationId);

        builder.HasOne(userIntegration => userIntegration.Integration)
            .WithMany();

        builder.HasMany(userIntegration => userIntegration.ConnectedRooms)
            .WithOne(connectedRoom => connectedRoom.UserIntegration);
    }
}
