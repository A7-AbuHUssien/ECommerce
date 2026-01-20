using System.ComponentModel.DataAnnotations;

namespace ECommerce.ViewModels;

public class ValidateOtpVM
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } 

    [Required(ErrorMessage = "OTP code is required.")]
    [Display(Name = "OTP Code")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "The OTP code must be 6 digits.")]
    public string OtpCode { get; set; }
}