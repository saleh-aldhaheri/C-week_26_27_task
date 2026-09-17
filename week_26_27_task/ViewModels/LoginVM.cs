using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace week_26_27.ViewModels;

public class LoginVM
{
    [Required]
    [DisplayName("Email Or Password")]
    public string EmailOrUserName { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
