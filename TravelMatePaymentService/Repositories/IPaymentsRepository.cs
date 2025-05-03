using TravelMatePaymentService.Models.Bookings;

namespace TravelMatePaymentService.Repositories;

public interface IPaymentsRepository
{
    Task<Payment> CreatePayment(Payment payment);
    Task<bool> ChangePaymentStatus(Guid paymentId, PaymentStatus status);
    Task<Payment> GetPaymentById(Guid paymentId);
}