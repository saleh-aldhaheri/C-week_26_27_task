using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace week_26_27.ViewModels;

public class LoginVM
{
    [Required]
    [DisplayName("Username or Email")]
    public string EmailOrUsername { get; set; }

    [Required]
    [DataType(DataType.EmailAddress)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
}
