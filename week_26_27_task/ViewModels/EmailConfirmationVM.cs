using System.ComponentModel.DataAnnotations;

namespace week_26_27.ViewModels;

public class EmailConfirmationVM
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;
}
