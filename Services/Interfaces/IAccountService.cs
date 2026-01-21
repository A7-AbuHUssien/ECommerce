using ECommerce.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Services.Interfaces;

public interface IAccountService
{
    Task<IdentityResult> RegisterAsync(RegisterVM model, string scheme, Func<string,string,string,string?,string?> generateLink);

    Task<SignInResult> LoginAsync(LoginVM model);

    Task<ApplicationUser?> GetUserByEmailOrUsernameAsync(string usernameOrEmail);

    Task SendEmailConfirmationAsync(ApplicationUser user, string scheme, Func<string,string,string,string?,string?> generateLink);
    Task<IdentityResult> ConfirmEmailAsync(string token, string userId);

    Task<bool> CanSendOTP(ApplicationUser user);
    Task<string> GenerateOTP(ApplicationUser user);
    Task<bool> ValidateOTP(string userId, string otpCode);

    Task<IdentityResult> ResetPasswordAsync(ResetPasswordVM model);
    Task LogoutAsync();
}
