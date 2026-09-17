using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace week_26_27.ViewModels;

public class RegisterVM
{
    [Required]
    [DisplayName("First Name")]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [DisplayName("Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    
    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password))]
    public string ConfirmationPassword { get; set; } = string.Empty;
}
