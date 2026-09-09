namespace week_26_27_task.ViewModels
{
    public class CategoryWithFilterAndPaginationVM
    {
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public PaginationVM Pagination { get; set; } = new PaginationVM();
        public string? Search { get; set; }
        public bool? Status { get; set; }
    }
}
