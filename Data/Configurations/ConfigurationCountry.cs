using CarPartsProject.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarPartsProject.Data.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<CarPartCountry>
    {
        public void Configure(EntityTypeBuilder<CarPartCountry> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.CountryName)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(c => c.CountryName).IsUnique();
        }
    }