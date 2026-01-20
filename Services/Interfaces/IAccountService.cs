using ECommerce.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Services.Interfaces;

public interface IAccountService
{
    // Register
    Task<IdentityResult> RegisterAsync(RegisterVM model, string scheme, Func<string,string,string,string?,string?> generateLink);

    // Login
    Task<SignInResult> LoginAsync(LoginVM model);

    // Get user by email or username
    Task<ApplicationUser?> GetUserByEmailOrUsernameAsync(string usernameOrEmail);

    // Email Confirmation
    Task SendEmailConfirmationAsync(ApplicationUser user, string scheme, Func<string,string,string,string?,string?> generateLink);
    Task<IdentityResult> ConfirmEmailAsync(string token, string userId);

    // OTP / Forgot Password
    Task<bool> CanSendOTP(ApplicationUser user);
    Task<string> GenerateOTP(ApplicationUser user);
    Task<bool> ValidateOTP(string userId, string otpCode);

    // Reset Password
    Task<IdentityResult> ResetPasswordAsync(ResetPasswordVM model);
    Task LogoutAsync();
}
