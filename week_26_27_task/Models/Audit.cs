namespace week_26_27.Models;

public class Audit
{
    public DateTime CreateAt { get; set; } = DateTime.UtcNow; 
    public DateTime? UpdateAt { get; set; }
    //public int CreatedById { get; set; }
    //public int UpdatedById { get; set; }
}
