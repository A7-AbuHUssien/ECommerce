using ECommerce.Services.Interfaces;
using ECommerce.Utilities;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{StaticData.SUPER_ADMIN_ROLE}")]
public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserController(IUserService userService, UserManager<ApplicationUser> userManager)
    {
        _userService = userService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllUsersAsync();
        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.RoleList = await _userService.GetAvailableRolesAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ApplicationUser user, string password, string selectedRole)
    {
        if (string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError("", "Password is required.");
            ViewBag.RoleList = await _userService.GetAvailableRolesAsync();
            return View(user);
        }

        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, selectedRole);

            TempData["success-notification"] = "User created with role: " + selectedRole;
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
        ViewBag.RoleList = await _userService.GetAvailableRolesAsync();
        return View(user);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();

        ViewBag.RoleList = await _userService.GetAvailableRolesAsync();
        ViewBag.CurrentUserRole = await _userService.GetUserRoleAsync(id);

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ApplicationUser user, string selectedRole)
    {
        var existingUser = await _userManager.FindByIdAsync(user.Id);
        if (existingUser == null) return NotFound();

        existingUser.Name = user.Name;
        existingUser.PhoneNumber = user.PhoneNumber;
        existingUser.Address = user.Address;


        var result = await _userManager.UpdateAsync(existingUser);
        if (result.Succeeded)
        {
            var oldRoles = await _userManager.GetRolesAsync(existingUser);
            await _userManager.RemoveFromRolesAsync(existingUser, oldRoles);
            await _userManager.AddToRoleAsync(existingUser, selectedRole);

            TempData["success-notification"] = "User and Role updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.RoleList = await _userService.GetAvailableRolesAsync();
        return View(user);
    }

    public async Task<IActionResult> LockUnlock(string id)
    {
        var result = await _userService.LockUnlockAsync(id);
        if (result.Succeeded)
        {
            TempData["success-notification"] = "User status updated successfully.";
        }
        else
        {
            TempData["error-notification"] = "Failed to update user status.";
        }

        return RedirectToAction(nameof(Index));
    }
}