using CarPartsProject.DTOs.Responses;
using CarPartsProject.DTOs.Requests.Users;
using CarPartsProject.Entities;
using CarPartsProject.Enums;

namespace CarPartsProject.Services.UserService;

public interface IUserService
{
    public Task<UserResponse> AddNewUserAsync(UserRequest request, Guid? CurrentUserId);
    public Task<UserResponse> UpdateUserAsync(User request, Guid? CurrentUserId);
    public Task<UserResponse> GetUserAsync(Guid UserId, UserRole UserRole);
    public Task<User> GetUserAsync(Guid UserId);
    public Task<User> GetUserByEmailPasswordAsync(string Email, string Password);
    public Task<IEnumerable<UserResponse>> GetUsersAsync(UserRole UserRole, int Page = 1, int PageSize = 10);
    public Task DeleteUserAsync(Guid UserId);
}