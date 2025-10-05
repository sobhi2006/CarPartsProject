namespace CarPartsProject.Entities;

public class CarPartImage
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public Guid CarPartId { get; set; }
    public CarPart CarPart { get; set; } = null!;
}