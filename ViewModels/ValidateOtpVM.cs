using System.ComponentModel.DataAnnotations;

namespace ECommerce.ViewModels;

public class ValidateOtpVM
{
    [Required(ErrorMessage = "OTP code is required.")]
    [Display(Name = "OTP Code")]
    [StringLength(4, MinimumLength = 4, ErrorMessage = "The OTP code must be 4 digits.")]
    public string OtpCode { get; set; }
    public string UserId { get; set; }
}