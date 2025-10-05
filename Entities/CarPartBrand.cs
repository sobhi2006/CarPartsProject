namespace CarPartsProject.Entities;

public class CarPartBrand
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<CarPart> Parts { get; set; } = [];
}