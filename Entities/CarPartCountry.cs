namespace CarPartsProject.Entities;

public class CarPartCountry
{
    public Guid Id { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public ICollection<CarPart> Parts { get; set; } = [];
}