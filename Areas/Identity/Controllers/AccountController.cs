using ECommerce.Repositories.Interfaces;
using ECommerce.Services.Interfaces;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Areas.Identity.Controllers;

[Area("Identity")]
public class AccountController : Controller
{
    private readonly IAccountService _accountService;
    
    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    // ----------------- Register -----------------
    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _accountService.RegisterAsync(model, Request.Scheme,
            (action, controller, userId, token) => Url.Action(action, controller,
                new { area = "Identity", userid = userId, token = token }, Request.Scheme));

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
            return View(model);
        }

        return RedirectToAction("Login");
    }

    // ----------------- Confirm Email -----------------
    public async Task<IActionResult> ConfirmEmail(string token, string userid)
    {
        var result = await _accountService.ConfirmEmailAsync(token, userid);
        if (result.Succeeded) return RedirectToAction("Login");
        return View("Error");
    }

    // ----------------- Login -----------------
    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _accountService.LoginAsync(model);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Invalid login attempt");
            return View(model);
        }

        return RedirectToAction("Index", "Home", new { area = "Customer" });
    }

    // ----------------- Forgot Password -----------------
    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _accountService.GetUserByEmailOrUsernameAsync(model.UsernameOrEmail);
        if (user == null)
        {
            ModelState.AddModelError("", "Invalid Email/Username");
            return View(model);
        }

        if (!await _accountService.CanSendOTP(user))
        {
            ModelState.AddModelError("", "Maximum OTPs reached for today");
            return View(model);
        }

        await _accountService.GenerateOTP(user);
        TempData["FromForgotPassword"] = Guid.NewGuid().ToString();
        
        return RedirectToAction("ValidateOTP", new { userid = user.Id });
    }

    // ----------------- Validate OTP -----------------
    [HttpGet]
    public IActionResult ValidateOTP(string userid)
    {
        if (TempData["FromForgotPassword"] == null) 
            return RedirectToAction("NotFoundPage", "Home", new { area = "Admin" });
        return View(new ValidateOtpVM { UserId = userid });
    }

    [HttpPost]
    public async Task<IActionResult> ValidateOTP(ValidateOtpVM otpVm)
    {
        if (!ModelState.IsValid)
            return RedirectToAction("ValidateOTP", new { userid = otpVm.UserId });

        bool isValid = await _accountService.ValidateOTP(otpVm.UserId, otpVm.OtpCode);
        if (!isValid)
        {
            ModelState.AddModelError("", "Invalid OTP");
            return RedirectToAction("ValidateOTP", new { userid = otpVm.UserId });
        }

        TempData["FromValidateOtp"] = Guid.NewGuid().ToString();
        return RedirectToAction("ResetPassword", new ResetPasswordVM { UserId = otpVm.UserId });
    }

    // ----------------- Reset Password -----------------
    [HttpGet]
    public IActionResult ResetPassword(string userid)
    {
        if (TempData["FromValidateOtp"] == null) 
            return RedirectToAction("NotFoundPage", "Home", new { area = "Admin" });

        return View(new ResetPasswordVM { UserId = userid });
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _accountService.ResetPasswordAsync(model);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Failed to reset password");
            return View(model);
        }

        return RedirectToAction("Login");
    }
    // ----------------- Logout -----------------
    public async Task<IActionResult> Logout()
    {
        await _accountService.LogoutAsync();
        return RedirectToAction("Login");
    }
   
}