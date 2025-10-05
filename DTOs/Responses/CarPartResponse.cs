using CarPartsProject.Entities;
using CarPartsProject.Enums;

namespace CarPartsProject.DTOs.Responses;

public class CarPartResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CarPartStatus StatusPart { get; set; }
    public string CountryPart { get; set; } = string.Empty;
    public string CarPartBrand { get; set; } = string.Empty;
    public ICollection<string> ImageUrls { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public static CarPartResponse FromEntity(CarPart CarPart)
    {
        return new()
        {
            Id = CarPart.Id,
            CarPartBrand = CarPart.Brand.Name,
            CountryPart = CarPart.CountryPart.CountryName,
            Description = CarPart.Description!,
            Name = CarPart.Name,
            CreatedAt = CarPart.CreatedAt,
            ImageUrls = CarPart.ImageUrls.Select(i => i.ImageUrl).ToList(),
            StatusPart = CarPart.StatusPart,
            UpdatedAt = CarPart.UpdatedAt
        };
    }
}