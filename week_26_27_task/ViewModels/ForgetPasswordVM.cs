using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace week_26_27.ViewModels;

public class ForgetPasswordVM
{
    [Required]
    [DisplayName("Email Or Username")]
    public string EmailOrPassword { get; set; } = string.Empty;
}
