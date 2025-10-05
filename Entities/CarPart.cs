using CarPartsProject.Enums;

namespace CarPartsProject.Entities;

public class CarPart
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CarPartStatus StatusPart { get; set; }
    public Guid CountryPartId { get; set; }
    public CarPartCountry CountryPart { get; set; } = null!;
    public Guid CreatedByUserId { get; set; }
    public User User { get; set; } = null!;
    public Guid CarPartBrandId { get; set; }
    public CarPartBrand Brand { get; set; } = null!;
    public ICollection<CarPartImage> ImageUrls { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}