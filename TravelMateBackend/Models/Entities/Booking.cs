using System.ComponentModel.DataAnnotations.Schema;

namespace TravelMateBackend.Models.Entities
{
    public class Booking
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public User User { get; set; }
        public Offer Offer { get; set; }
        public Payment Payment { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime ReservedUntil { get; set; }
        public string SeatNumber { get; set; }
        public SeatType SeatType { get; set; }
        public string PassengerName { get; set; }
        public string PassengerType { get; set; }
        public DateTime CreatedAt { get; set; }


    }
    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Canceled
    }

    public enum SeatType
    {
        Economy,
        Business,
        FirstClass
    }

    public enum PassengerType
    {
        Adult,
        Child,
        Baby
    }
}
