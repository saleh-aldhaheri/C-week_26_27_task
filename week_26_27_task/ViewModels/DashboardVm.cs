namespace week_26_27_task.ViewModels;

public class DashboardVm
{
    public int MoviesCount { get; set; }
    public int ActorsCount { get; set; }
    public int CategoriesCount { get; set; }
    public int CinemasCount { get; set; }
    public List<MoviePricePointVm> MoviePrices { get; set; } = new();
}
