namespace week_26_27.Models;

public class Cart
{
    public int Id { get; set; }

    public string ApplicationUserId { get; set; } = string.Empty;

    public decimal TotalPrice { get; set; }

    public int Count { get; set; } // Number of tickets

    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;

    public ICollection<CartSeat> CartSeats { get; set; } = new List<CartSeat>();
}