using System.ComponentModel.DataAnnotations;

namespace week_26_27.ViewModels;

public class NewPasswordVM
{
    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;
}
