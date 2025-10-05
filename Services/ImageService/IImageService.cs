namespace CarPartsProject.Services.ImageService;

public interface IImageService
{
    Task<List<string>> SaveImagesAsync(List<IFormFile> images, string partName, string brand);
    bool DeleteImageAsync(string imageUrl);
    bool DeletePartFolderAsync(string partName);
}