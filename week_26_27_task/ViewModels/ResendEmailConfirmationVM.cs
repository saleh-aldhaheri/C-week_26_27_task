using System.ComponentModel.DataAnnotations;

namespace week_26_27.ViewModels; 

public class ResendEmailConfirmationVM
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string EmailOrUsername { get; set; } = string.Empty; 
}