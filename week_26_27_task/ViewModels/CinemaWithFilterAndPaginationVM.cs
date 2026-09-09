namespace week_26_27_task.ViewModels
{
    public class CinemaWithFilterAndPaginationVM
    {
        public IEnumerable<Cinema> Cinemas { get; set; } = new List<Cinema>();
        public PaginationVM Pagination { get; set; } = new PaginationVM();
        public string? Search { get; set; }
    }
}
