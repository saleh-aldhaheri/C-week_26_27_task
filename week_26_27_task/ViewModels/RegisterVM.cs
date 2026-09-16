using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace week_26_27.ViewModels;
public class RegisterVM
{
    [Required]
    [MinLength(2)]
    [DisplayName("First Name")]
    public string FirstName { get; set; }
    [Required]
    [MinLength(2)]
    [DisplayName("Last Name")]
    public string LastName { get; set; }
    [Required]
    [DisplayName("User Name")]
    public string UserName { get; set; }
    [Required]
    [DisplayName("Phone Number")]
    [DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; }
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [Required]
    [Compare(nameof(ConfirmationPassword))]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string ConfirmationPassword { get; set; }
}
