using System.ComponentModel.DataAnnotations;

namespace week_26_27.ViewModels;

public class SendEmailConfirmationVM
{
    [Required]
    public string EmailOrUserName { get; set;  } = string.Empty;
}
