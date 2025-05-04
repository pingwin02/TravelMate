using TravelMatePaymentService.Models.Bookings;

namespace TravelMatePaymentService.Services;

public interface IPaymentService
{
    Task<Payment> GetPaymentById(Guid paymentId);
    Task<Payment> CreatePayment(Guid bookingId, decimal price);
    Task<bool> FinalizePayment(Guid paymentId);
}