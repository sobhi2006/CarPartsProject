using System.Threading.Tasks;
using CarPartsProject.Data;
using CarPartsProject.DTOs.Requests.CarParts;
using CarPartsProject.DTOs.Responses;
using CarPartsProject.Entities;
using CarPartsProject.Enums;
using CarPartsProject.Exceptions;
using CarPartsProject.Services.ImageService;
using Microsoft.EntityFrameworkCore;

namespace CarPartsProject.Services.PartService;

public class PartService(AppDbContext context, IImageService imageService) : IPartService
{
    public async Task<List<CarPartResponse>> GetAllPartsAsync(int Page, int PageSize)
    {
        var parts = await context.Parts.Include(p => p.CountryPart)
                                       .Include(p => p.Brand)
                                       .Include(p => p.ImageUrls)
                                       .Skip((Page - 1) * PageSize)
                                       .Take(PageSize)
                                       .ToListAsync();
        return parts.Select(CarPartResponse.FromEntity).ToList();
    }
    public async Task<List<CarPartResponse>> GetRandomPartsAsync(int count)
    {
        var parts = await context.Parts.Include(p => p.CountryPart)
                                       .Include(p => p.Brand)
                                       .Include(p => p.ImageUrls)
                                       .OrderBy(x => Guid.NewGuid())
                                       .Take(count)
                                       .ToListAsync();

        return parts.Select(CarPartResponse.FromEntity).ToList();
    }

    public async Task<List<CarPartResponse>> GetPartsByStatusAsync(string Status, int Page, int PageSize)
    {
        if (!Enum.TryParse<CarPartStatus>(Status.Trim(), true, out var StatusEnum))
            return new List<CarPartResponse>();

        var parts = await context.Parts.Include(p => p.CountryPart)
                                       .Include(p => p.Brand)
                                       .Include(p => p.ImageUrls)
                                       .Where(p => p.StatusPart == StatusEnum)
                                       .Skip((Page - 1) * PageSize)
                                       .Take(PageSize)
                                       .ToListAsync();

        return parts.Select(CarPartResponse.FromEntity).ToList();
    }

    public async Task<List<CarPartResponse>> GetPartsByCountryAsync(string CountryName, int Page, int PageSize)
    {
        var parts = await context.Parts.Include(p => p.CountryPart)
                                       .Include(p => p.Brand)
                                       .Include(p => p.ImageUrls)
                                       .Where(p => p.CountryPart.CountryName.Contains(CountryName.Trim().ToLower()))
                                       .Skip((Page - 1) * PageSize)
                                       .Take(PageSize)
                                       .ToListAsync();

        return parts.Select(CarPartResponse.FromEntity).ToList();
    }

    public async Task<List<CarPartResponse>> GetPartsByBrandAsync(string brand, int Page, int PageSize)
    {
        var parts = await context.Parts.Include(p => p.CountryPart)
                                       .Include(p => p.Brand)
                                       .Include(p => p.ImageUrls)
                                       .Where(p => p.Brand.Name.ToLower().Contains(brand.Trim().ToLower()))
                                       .Skip((Page - 1) * PageSize)
                                       .Take(PageSize)
                                       .ToListAsync();

        return parts.Select(CarPartResponse.FromEntity).ToList();
    }

    public async Task<List<CarPartResponse>> SearchPartsAsync(string keyword)
    {
        keyword = keyword.Trim();
        var parts = await context.Parts.Include(p => p.CountryPart)
                                       .Include(p => p.Brand)
                                       .Include(p => p.ImageUrls)
                                       .Where(p => p.Name.ToLower().Contains(keyword.Trim().ToLower()))
                                       .ToListAsync();

        return parts.Select(CarPartResponse.FromEntity).ToList();
    }

    public async Task<CarPartResponse> CreatePartAsync(CarPartRequest request, Guid CurrentUserId)
    {
        if (await context.Parts.AnyAsync(p => p.Brand.Name == request.Brand && p.Name == request.Name))
            throw new BusinessRuleException("Part is found, you can not added this part.", StatusCodes.Status409Conflict);

        List<string> imageUrls = new List<string>();
        if (request.ImageFile != null && request.ImageFile.Any())
        {
            imageUrls = await imageService.SaveImagesAsync(request.ImageFile, request.Name, request.Brand);
        }
       
        var part = new CarPart
        {
            Name = request.Name,
            Description = request.Description,
            StatusPart = request.StatusPart,
            CountryPartId = await this.GetCountryIdByName(request.CountryPart),
            CarPartBrandId = await this.GetBrandIdByName(request.Brand),
            ImageUrls = imageUrls.Select(i => new CarPartImage { Id = Guid.NewGuid(), ImageUrl = i }).ToList(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedByUserId = CurrentUserId
        };        

        context.Parts.Add(part);
        await context.SaveChangesAsync();
        part.CountryPart = new CarPartCountry { CountryName = request.CountryPart };
        part.Brand = new CarPartBrand { Name = request.Brand };

        return CarPartResponse.FromEntity(part);
    }

    public async Task<CarPartResponse> UpdatePartAsync(Guid id, CarPartRequest request, Guid CurrentUserId)
    {
        if (await context.Parts.AnyAsync(p => p.Brand.Name == request.Brand && p.Name == request.Name && p.Id != id))
            throw new BusinessRuleException("Part is found, you can not added this part.", StatusCodes.Status409Conflict);

        var part = await context.Parts.Include(p => p.Brand)
                                      .Include(p => p.CountryPart)
                                      .Include(p => p.ImageUrls)
                                      .FirstOrDefaultAsync(p => p.Id == id) ??
            throw new BusinessRuleException("Part is not found.", StatusCodes.Status404NotFound);

        List<string> imageUrls = new List<string>();
        if (request.ImageFile != null && request.ImageFile.Any())
        {
            imageUrls = await imageService.SaveImagesAsync(request.ImageFile, request.Name, request.Brand);
        }

        part.Name = request.Name;
        part.Description = request.Description;
        part.StatusPart = request.StatusPart;
        part.CountryPartId = await this.GetCountryIdByName(request.CountryPart);
        part.CarPartBrandId = await this.GetBrandIdByName(request.Brand);
                               
        part.ImageUrls = imageUrls.Select(i => new CarPartImage{ImageUrl = i}).ToList();
        part.UpdatedAt = DateTime.UtcNow;
        part.CreatedByUserId = CurrentUserId;

        await context.SaveChangesAsync();
        part.CountryPart = new CarPartCountry { CountryName = request.CountryPart };
        part.Brand = new CarPartBrand { Name = request.Brand };
        return CarPartResponse.FromEntity(part);
    }

    public async Task DeletePartAsync(Guid id)
    {
        var part = await context.Parts.Include(p => p.Brand).FirstOrDefaultAsync(p => p.Id == id) ??
            throw new BusinessRuleException("Part is not found.", StatusCodes.Status404NotFound);

        imageService.DeletePartFolderAsync(part.Name + '-' + part.Brand.Name);
        context.Parts.Remove(part);
        await context.SaveChangesAsync();
    }

    private async Task<Guid> GetCountryIdByName(string CountryName)
    {
        var Country = await context.Countries.AsNoTracking().FirstOrDefaultAsync(c => c.CountryName ==
                                                CountryName.Trim().ToLower());

        if (Country is null)
        {
            Country = new CarPartCountry { Id = Guid.NewGuid(), CountryName = CountryName.Trim().ToLower() };
            context.Countries.Add(Country);
            await context.SaveChangesAsync();
        }
        return Country.Id;
    }
    
    private async Task<Guid> GetBrandIdByName(string BrandName)
    {
        var Brand = await context.Brands.AsNoTracking()
                                        .FirstOrDefaultAsync(c => c.Name == BrandName.Trim().ToLower());

        if (Brand is null)
        {
            Brand = new CarPartBrand { Id = Guid.NewGuid(), Name = BrandName.Trim().ToLower() };
            context.Brands.Add(Brand);
            await context.SaveChangesAsync();
        }
        return Brand.Id;
    }
}
