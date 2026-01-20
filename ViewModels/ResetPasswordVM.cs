using System.ComponentModel.DataAnnotations;

namespace ECommerce.ViewModels;

public class ResetPasswordVM
{
    public string UserId { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required,DataType(DataType.Password),Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }
}