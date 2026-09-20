namespace week_26_27_task.ViewModels;

public class PaginationVM
{
   public int TotalPages { get; set; }
   public int PageSize { get; set; } = 15;
   public int Page { get; set; } = 1;
    public int TotalItems { get; set; } = 0;
}
