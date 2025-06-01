using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataContext.Configuration;

internal class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Name)
            .IsRequired();
        builder.HasIndex(entity => entity.Name)
            .IsUnique();
    }
}
