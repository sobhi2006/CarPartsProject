using CarPartsProject.Entities;
using CarPartsProject.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CarPartsProject.Data.Configurations;

public class PartConfiguration : IEntityTypeConfiguration<CarPart>
{
    public void Configure(EntityTypeBuilder<CarPart> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        var converter = new EnumToStringConverter<CarPartStatus>();
        builder.Property(p => p.StatusPart)
            .HasConversion(converter)
            .HasMaxLength(20);

        builder.HasIndex(p => p.StatusPart);
        builder.HasIndex(p => p.CountryPartId);
        builder.HasIndex(p => p.CarPartBrandId);
        builder.HasIndex(p => p.Name);

        builder.HasOne<CarPartCountry>(p => p.CountryPart).WithMany(c => c.Parts).HasForeignKey(p => p.CountryPartId);
        builder.HasOne<User>(p => p.User).WithMany(u => u.CarParts).HasForeignKey(p => p.CreatedByUserId);
        builder.HasOne<CarPartBrand>(p => p.Brand).WithMany(b => b.Parts).HasForeignKey(p => p.CarPartBrandId);
    }
}