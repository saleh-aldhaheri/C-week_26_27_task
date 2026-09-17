using System.ComponentModel.DataAnnotations;

namespace week_26_27.ViewModels;

public class ValidateOtpVM
{
    [Required]
    [MaxLength(6)]
    public string Otp { get; set; } = string.Empty;
}
