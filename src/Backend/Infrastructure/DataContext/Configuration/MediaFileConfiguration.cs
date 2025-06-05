using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataContext.Configuration;

internal class MediaFileConfiguration : IEntityTypeConfiguration<MediaFile>
{
    public void Configure(EntityTypeBuilder<MediaFile> builder)
    {
        builder.HasKey(file => file.Id);

        builder.Property(file => file.Url)
            .IsRequired();
        builder.HasIndex(file => file.Url)
            .IsUnique();

        builder.Property(file => file.FilePath)
            .IsRequired();
        builder.HasIndex(file => file.FilePath)
            .IsUnique();
    }
}
