using CarPartsProject.DTOs.Requests.CarParts;
using CarPartsProject.DTOs.Responses;

namespace CarPartsProject.Services.PartService;

public interface IPartService
{
    Task<List<CarPartResponse>> GetAllPartsAsync(int Page, int PageSize);
    Task<List<CarPartResponse>> GetRandomPartsAsync(int count);
    Task<List<CarPartResponse>> GetPartsByStatusAsync(string status, int Page, int PageSize);
    Task<List<CarPartResponse>> GetPartsByBrandAsync(string brand, int Page, int PageSize);
    Task<List<CarPartResponse>> GetPartsByCountryAsync(string CountryName, int Page, int PageSize);
    Task<List<CarPartResponse>> SearchPartsAsync(string Name);
    Task<CarPartResponse> CreatePartAsync(CarPartRequest request, Guid CurrentUserId);
    Task<CarPartResponse> UpdatePartAsync(Guid id, CarPartRequest request, Guid CurrentUserId);
    Task DeletePartAsync(Guid id);
}