using CarPartsProject.Entities;
using CarPartsProject.Enums;

namespace CarPartsProject.DTOs.Requests.CarParts;

public class CarPartRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CarPartStatus StatusPart { get; set; }
    public string CountryPart { get; set; } = null!;
    public string Brand { get; set; } = null!;
    public List<IFormFile> ImageFile { get; set; } = [];
}