using ECommerce.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
    Task<ApplicationUser?> GetByIdAsync(string userId);
    Task<IdentityResult> UpdateProfileAsync(ApplicationUserVM model);
    Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    Task<IdentityResult> DeleteAsync(string userId);
    // Added for Admin Create functionality
    Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
    Task<List<string>> GetAvailableRolesAsync();
    Task<string> GetUserRoleAsync(string userId);
    Task<IdentityResult> LockUnlockAsync(string userId);
}