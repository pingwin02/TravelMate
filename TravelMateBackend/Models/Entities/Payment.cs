using System.ComponentModel.DataAnnotations.Schema;

namespace TravelMateBackend.Models.Entities
{
    public class Payment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid BookingId { get; set; } //FK, bo to 1-1 relacja
        public Booking Booking { get; set; }
        public decimal Price { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed
    }
}
