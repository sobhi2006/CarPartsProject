using CarPartsProject.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarPartsProject.Data.Configurations;

public class BrandConfiguration : IEntityTypeConfiguration<CarPartBrand>
{
    public void Configure(EntityTypeBuilder<CarPartBrand> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(b => b.Name).IsUnique();
    }
}