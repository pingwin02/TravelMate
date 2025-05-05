using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelMatePaymentService.Models.Bookings;

public class Payment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public Guid BookingId { get; set; }
    public Guid CorrelationId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime TransactionDate { get; set; }
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed
}

public enum BookingStatus
{
    Pending,
    Confirmed,
    Canceled
}