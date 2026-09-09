namespace week_26_27_task.Helpers;
public class Pagination : IPagination
{
    public ViewModels.PaginationVM Paginate(int count = 0, int size = 15, int page = 1)
    {
        return  new()
        {
            PageSize = size,
            Page = page,
            TotalItems = count,
            TotalPages = (int)Math.Ceiling((double)count / (size))
        };
    }
}
