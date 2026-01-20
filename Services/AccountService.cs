using ECommerce.Areas.Identity.Controllers;
using ECommerce.Repositories.Interfaces;
using ECommerce.Services.Interfaces;
using ECommerce.Utilities;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace ECommerce.Services;
public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly IRepository<ApplicationUserOTP> _otpRepository;

    public AccountService(UserManager<ApplicationUser> userManager,
                          SignInManager<ApplicationUser> signInManager,
                          IEmailSender emailSender,
                          IRepository<ApplicationUserOTP> otpRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _otpRepository = otpRepository;
    }

    public async Task<ApplicationUser?> GetUserByEmailOrUsernameAsync(string usernameOrEmail)
    {
        return await _userManager.FindByEmailAsync(usernameOrEmail)
            ?? await _userManager.FindByNameAsync(usernameOrEmail);
    }

    // Register
    public async Task<IdentityResult> RegisterAsync(RegisterVM model, string scheme, Func<string,string,string,string?,string?> generateLink)
    {
        var user = new ApplicationUser { Name = model.UserName, UserName = model.UserName, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded) return result;

        await SendEmailConfirmationAsync(user, scheme, generateLink);
        await _userManager.AddToRoleAsync(user, StaticData.CUSTOMER_ROLE);
        return result;
    }

    public async Task SendEmailConfirmationAsync(ApplicationUser user, string scheme, Func<string,string,string,string?,string?> generateLink)
    {
        string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        string? link = generateLink(nameof(AccountController.ConfirmEmail), "Account", user.Id, token);
        await _emailSender.SendEmailAsync(user.Email, "ECommerce Alert",
            $"<h1>Confirm Your Email By Clicking <a href='{link}'>Here</a></h1>");
    }

    public async Task<IdentityResult> ConfirmEmailAsync(string token, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return IdentityResult.Failed();
        return await _userManager.ConfirmEmailAsync(user, token);
    }

    // Login
    public async Task<SignInResult> LoginAsync(LoginVM model)
    {
        var user = await GetUserByEmailOrUsernameAsync(model.UsernameOrEmail);
        if (user == null) return SignInResult.Failed;

        return await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);
    }

    // OTP / Forgot Password
    public async Task<bool> CanSendOTP(ApplicationUser user)
    {
        var otps = await _otpRepository.GetAsync(e => e.Id == user.Id);
        return otps.Count(e => (DateTime.UtcNow - e.CreatedAt).TotalHours < 24) < 3;
    }

    public async Task<string> GenerateOTP(ApplicationUser user)
    {
        var otp = new Random().Next(1000, 9999).ToString();
        await _otpRepository.CreateAsync(new ApplicationUserOTP
        {
            ApplicationUser = user,
            OTP = otp,
            CreatedAt = DateTime.UtcNow,
            IsValid = true,
            Id = Guid.NewGuid().ToString()
        });
        await _otpRepository.CommitAsync();
        await _emailSender.SendEmailAsync(user.Email, "ECommerce Alert", $"<h1>OTP: {otp}</h1>");
        return otp;
    }

    public async Task<bool> ValidateOTP(string userId, string otpCode)
    {
        var validOtp = (await _otpRepository.GetAsync(e => e.ApplicationUserId == userId))
                        .OrderBy(e => e.CreatedAt).LastOrDefault();
        return validOtp != null && validOtp.OTP == otpCode;
    }

    // Reset Password
    public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordVM model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null) return IdentityResult.Failed();

        string token = await _userManager.GeneratePasswordResetTokenAsync(user);
        return await _userManager.ResetPasswordAsync(user, token, model.Password);
    }
    
    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}
