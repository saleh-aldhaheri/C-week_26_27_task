using System.ComponentModel.DataAnnotations;

namespace week_26_27.ViewModels;

public class ForgetPasswordVM
{
    [Required]
    public string EmailOrUsername { get; set; } = string.Empty;
}
