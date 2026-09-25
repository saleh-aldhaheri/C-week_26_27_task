namespace week_26_27.Models;

public class Seat
{
    public int Id { get; set; }

    public int Row { get; set; }

    public int Column { get; set; }

    public int AuditoriumId { get; set; }
    public Auditorium Auditorium { get; set; } = null!;

    ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    ICollection<CartSeat> CartSeats { get; set; } = new List<CartSeat>();
}