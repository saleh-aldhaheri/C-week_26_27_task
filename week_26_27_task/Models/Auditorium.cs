using week_26_27.Models;

namespace week_26_27_task.ViewModels;
public class Auditorium
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; 
    public int CinemaId { get; set; }
    public Cinema Cinema { get; set; } = null!;
    public int Rows { get; set; } 
    public int Columns { get; set; }
    public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}