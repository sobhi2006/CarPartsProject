using CarPartsProject.Enums;

namespace CarPartsProject.Entities;

public class User
{
    public Guid Id { get; set; }
    public string FName { get; set; } = string.Empty;
    public string LName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public ICollection<CarPart> CarParts { get; set; } = [];
}