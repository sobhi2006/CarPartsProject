
namespace CarPartsProject.Services.ImageService;

public class ImageService : IImageService
{
    private readonly string _baseFolder = "images\\parts";
    public async Task<List<string>> SaveImagesAsync(List<IFormFile> images, string partName, string brand)
    {
        var savedUrls = new List<string>();
            
        var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var imagesPath = Path.Combine(webRootPath, "images", "parts", partName + '-' + brand);
        
        if(!Directory.Exists(imagesPath))
            Directory.CreateDirectory(imagesPath);
        
        foreach (var image in images)
        {
            if (image.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var filePath = Path.Combine(imagesPath, fileName);
                
                using var stream = new FileStream(filePath, FileMode.Create);
                await image.CopyToAsync(stream);
                
                savedUrls.Add($"/images/parts/{partName}-{brand}/{fileName}");
            }
        }
        
        return savedUrls;
    }

    public bool DeleteImageAsync(string imageUrl)
    {
        try
        {
            var fileName = Path.GetFileName(imageUrl);
            var partName = Path.GetFileName(Path.GetDirectoryName(imageUrl));
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "parts", partName!, fileName);
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public bool DeletePartFolderAsync(string partName)
    {
        try
        {
            var partFolderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "images",
                "parts",
                partName
            );

            if (Directory.Exists(partFolderPath))
            {
                Directory.Delete(partFolderPath, true);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting folder for part {partName}: {ex.Message}");
            return false;
        }
    }
}
