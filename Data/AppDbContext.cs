using CarPartsProject.Data.Configurations;
using CarPartsProject.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarPartsProject.Data;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{

    public DbSet<User> Users => Set<User>();
    public DbSet<CarPart> Parts => Set<CarPart>();
    public DbSet<CarPartCountry> Countries => Set<CarPartCountry>();
    public DbSet<CarPartBrand> Brands => Set<CarPartBrand>();
    public DbSet<CarPartImage> Images => Set<CarPartImage>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
    }
}