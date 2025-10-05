using CarPartsProject.Entities;

namespace CarPartsProject.DTOs.Responses;

public class AuthTokenResponse
{
    public string FullName { get; set; } = string.Empty;
    public string? AccessToken { get; set; }
    public DateTime Expired { get; set; }
    public string? RefreshToken { get; set; }

    public static AuthTokenResponse FromEntity(User user, string Key)
    {
        return new()
        {
            FullName = user.FName + " " + user.LName,
            RefreshToken = CryptographyData.Decrypt(user.RefreshToken!, Key),
            Expired = user.RefreshTokenExpiryTime
        };
    }
}