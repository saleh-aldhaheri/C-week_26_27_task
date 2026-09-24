namespace week_26_27.Models;
public class CartSeat
{
    public int Id { get; set; }

    public int CartId { get; set; }

    public Cart Cart { get; set; } = null!;

    public int SeatId { get; set; }

    public Seat Seat { get; set; } = null!;

    public DateTime ExpiredAt { get; set; } // User cannot reserve seat forever
}