namespace week_26_27.Models;
public class Auditorium
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty; 

    public int CinemaId { get; set; }
    public Cinema Cinema { get; set; } = null!;


    public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}