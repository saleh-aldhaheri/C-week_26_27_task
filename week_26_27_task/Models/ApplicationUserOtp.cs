using System.Security.AccessControl;

namespace week_26_27.Models;

public class ApplicationUserOtp : Audit
{
    public int Id { get; set; }
    public string Otp { get; set; } = string.Empty;
    public bool IsUsed { get; set; } = false; 
    public DateTime ExpiredAt { get; set; } = DateTime.UtcNow.AddMinutes(10);
    public string ApplicationUserId { get; set; } = string.Empty;
    public ApplicationUser applicationUser { get; set; } = null!;
}
