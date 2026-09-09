namespace week_26_27_task.Helpers.IHelpers;

public interface IPagination
{
    public ViewModels.PaginationVM Paginate(int count = 0, int size = 15, int page = 1); 
}
