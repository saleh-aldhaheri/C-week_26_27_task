namespace week_26_27_task.ViewModels
{
    public class ViewMovieVm
    {
        public Movie Movie { get; set; } = null!;  
        public IEnumerable<Actor>? Actors { get; set; }
    }
}
