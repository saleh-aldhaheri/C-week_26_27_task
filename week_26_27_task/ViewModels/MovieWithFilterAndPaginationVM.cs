namespace week_26_27_task.ViewModels;
public class MovieWithFilterAndPaginationVM
{
    public IEnumerable<Movie>? Movies { get; set; }
    public IEnumerable<Cinema>? Cinemas { get; set; }
    public IEnumerable<Category>? Categorias { get; set; }
    public PaginationVM Pagination { get; set; } = new PaginationVM();
    public int? CinemaId { get; set; }
    public int? CategoriasId { get; set; }
    public string? Search { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public DateTime? StartAt { get; set; }
}
