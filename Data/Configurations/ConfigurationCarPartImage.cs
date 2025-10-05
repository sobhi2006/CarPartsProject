using CarPartsProject.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarPartsProject.Data.Configurations;

public class ConfigurationCarPartImage : IEntityTypeConfiguration<CarPartImage>
{
    public void Configure(EntityTypeBuilder<CarPartImage> builder)
    {
        builder.HasOne<CarPart>(p => p.CarPart).WithMany(i => i.ImageUrls).HasForeignKey(i => i.CarPartId);
    }
}