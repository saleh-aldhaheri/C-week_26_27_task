using System.ComponentModel.DataAnnotations;

namespace week_26_27.ViewModels;

public class NewPasswordVM
{
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;
}
