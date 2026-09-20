namespace week_26_27_task.ViewModels;

public class ActorWithFilterAndPaginationVM
{
    public IEnumerable<Actor> Actors { get; set; } = new List<Actor>();
    public PaginationVM Pagination { get; set; } = new PaginationVM();
    public string? Search { get; set; }
}
