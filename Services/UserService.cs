using ECommerce.Services.Interfaces;
using ECommerce.Utilities;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
    {
        return await _userManager.Users.AsNoTracking().ToListAsync();
    }

    public async Task<ApplicationUser?> GetByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<IdentityResult> UpdateProfileAsync(ApplicationUserVM model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });

        user.Name = model.Name;
        user.PhoneNumber = model.PhoneNumber;
        user.Address = model.Address;

        return await _userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });

        return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
    }

    public async Task<IdentityResult> DeleteAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });

        return await _userManager.DeleteAsync(user);
    }

    public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }
    public async Task<List<string>> GetAvailableRolesAsync()
    {
        return new List<string> 
        { 
            StaticData.SUPER_ADMIN_ROLE, 
            StaticData.ADMIN_ROLE, 
            StaticData.EMPLOYEE_ROLE, 
            StaticData.CUSTOMER_ROLE 
        };
    }

    public async Task<string> GetUserRoleAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return string.Empty;
        var roles = await _userManager.GetRolesAsync(user);
        return roles.FirstOrDefault() ?? string.Empty;
    }
    public async Task<IdentityResult> LockUnlockAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) 
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        
        if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
            user.LockoutEnd = DateTime.UtcNow;
        else
            user.LockoutEnd = DateTime.UtcNow.AddYears(100);
        
        if (_userManager.IsInRoleAsync(user, StaticData.SUPER_ADMIN_ROLE).Result)
            return IdentityResult.Failed();
        
        return await _userManager.UpdateAsync(user);
    }
}