namespace week_26_27.Models;

public class Seat
{
    public int Id { get; set; }

    public string Row { get; set; } = string.Empty;

    public int Column { get; set; }

    public int AuditoriumId { get; set; }
    public Auditorium Auditorium { get; set; } = null!;
}