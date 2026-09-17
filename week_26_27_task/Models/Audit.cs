namespace week_26_27.Models;

public class Audit
{
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdateAt { get; set; }
}
