using System.ComponentModel.DataAnnotations;

namespace ECommerce.ViewModels;

public class ForgotPasswordVM
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    public string UsernameOrEmail { get; set; }
}