using MassTransit;
using Microsoft.Extensions.Options;
using TravelMate.Models.Messages;
using TravelMatePaymentService.Models.Bookings;
using TravelMatePaymentService.Models.Settings;
using TravelMatePaymentService.Repositories;

namespace TravelMatePaymentService.Services;

public class PaymentService(
    IPaymentsRepository paymentsRepository,
    IOptions<PaymentsSettings> settings,
    IRequestClient<BookingStatusUpdateRequest> bookingStatusUpdateRequest)
    : IPaymentService
{
    public async Task<Payment> GetPaymentById(Guid paymentId)
    {
        return await paymentsRepository.GetPaymentById(paymentId);
    }

    public async Task<Payment> CreatePayment(Guid bookingId, decimal price)
    {
        var payment = new Payment
        {
            BookingId = bookingId,
            Amount = price,
            Status = PaymentStatus.Pending
        };

        return await paymentsRepository.CreatePayment(payment);
    }

    public async Task<bool> FinalizePayment(Guid paymentId)
    {
        var payment = await paymentsRepository.GetPaymentById(paymentId);

        if (payment.Status != PaymentStatus.Pending)
            throw new InvalidOperationException($"Payment with id {paymentId} is not in pending status");

        var isSuccess = new Random().NextDouble() > settings.Value.PaymentFailureChance;

        await paymentsRepository.ChangePaymentStatus(paymentId,
            isSuccess ? PaymentStatus.Completed : PaymentStatus.Failed);

        var bookingStatusUpdateResponse = await bookingStatusUpdateRequest.GetResponse<BookingStatusUpdateResponse>(
            new BookingStatusUpdateRequest
            {
                BookingId = payment.BookingId,
                Status = isSuccess ? BookingStatus.Confirmed : BookingStatus.Canceled
            });

        Console.WriteLine(bookingStatusUpdateResponse.Message.IsUpdated
            ? "Booking status updated successfully"
            : "Failed to update booking status");

        return isSuccess;
    }
}