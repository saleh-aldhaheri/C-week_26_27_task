namespace week_26_27.Models;

public enum BookingStatus
{
    Pending, 
    Canceled, 
    Completed
}

public enum PaymentMethod
{
  Stripe, 
  Paypal, 
  MasterCard, 
  Visa
}

public enum PaymentStatus
{
    Pending,
    Succussed,
    Canceled,
    Refused,
    Refunded
}

public class Booking : Audit
{
    public int Id { get; set; }

    public string ApplicationUserId { get; set; } = string.Empty;

    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;

    public BookingStatus BookingStatus { get; set; } = BookingStatus.Pending;

    public decimal TotalPrice { get; set; }
    
    public PaymentMethod PaymentMethod { get; set; } =  PaymentMethod.Stripe;

    public PaymentStatus PaymentStatus { get; set; }

    public string? TransactionId { get; set; }

    public string? SessionId { get; set; }

    public DateTime PaymentDate { get; set; }

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}