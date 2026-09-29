using week_26_27.Models;
using week_26_27_task.Models;

namespace week_26_27_task.ViewModels;

public class CustomerMovieShowVM
{
    public  Movie Movie { get; set; } = null!;
    public Auditorium Auditorium { get; set; } = null!;
}
