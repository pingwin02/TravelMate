using TravelMatePaymentService.Data;
using TravelMatePaymentService.Models.Bookings;

namespace TravelMatePaymentService.Repositories;

public class PaymentsRepository(DataContext dataContext) : IPaymentsRepository
{
    public async Task<Payment> CreatePayment(Payment payment)
    {
        await dataContext.Payments.AddAsync(payment);
        await dataContext.SaveChangesAsync();
        return payment;
    }

    public async Task<bool> ChangePaymentStatus(Guid paymentId, PaymentStatus status)
    {
        var payment = await dataContext.Payments.FindAsync(paymentId);

        if (payment == null)
            throw new KeyNotFoundException($"Payment with id {paymentId} not found");

        payment.Status = status;
        payment.TransactionDate = DateTime.Now;

        dataContext.Payments.Update(payment);
        await dataContext.SaveChangesAsync();
        return true;
    }

    public async Task<Payment> GetPaymentById(Guid paymentId)
    {
        var payment = await dataContext.Payments.FindAsync(paymentId);

        if (payment == null)
            throw new KeyNotFoundException($"Payment with id {paymentId} not found");

        return payment;
    }
}