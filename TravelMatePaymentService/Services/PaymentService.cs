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
    IRequestClient<BookingStatusUpdateRequest> bookingStatusUpdateRequest,
    IPublishEndpoint publishEndpoint)
    : IPaymentService
{
    public async Task<Payment> GetPaymentById(Guid paymentId)
    {
        return await paymentsRepository.GetPaymentById(paymentId);
    }

    public async Task<Payment> CreatePayment(Guid bookingId, decimal price, Guid correlationId)
    {
        var payment = new Payment
        {
            BookingId = bookingId,
            Amount = price,
            Status = PaymentStatus.Pending,
            CorrelationId = correlationId
        };

        return await paymentsRepository.CreatePayment(payment);
    }

    public async Task<bool> FinalizePayment(Guid paymentId)
    {
        var payment = await paymentsRepository.GetPaymentById(paymentId);

        if (payment.Status != PaymentStatus.Pending)
            throw new InvalidOperationException($"Payment with id {paymentId} is not in pending status");

        var isSuccess = new Random().NextDouble() > settings.Value.PaymentFailureChance;

        var status = isSuccess ? PaymentStatus.Completed : PaymentStatus.Failed;
        await paymentsRepository.ChangePaymentStatus(paymentId, status);
        Console.WriteLine($"Payment status updated for payment {paymentId} to {status}");

        if(isSuccess)
        {
            await publishEndpoint.Publish(new PaymentFinalizedEvent
            {
                CorrelationId = payment.CorrelationId,
                IsSuccess = isSuccess,
            });
        }
        else
        {
            await publishEndpoint.Publish(new PaymentFailedEvent
            {
                CorrelationId = payment.CorrelationId,
            });
        }

        return isSuccess;
    }

    public async Task<bool> CancelPayment(Guid paymentId)
    {
        var payment = await paymentsRepository.GetPaymentById(paymentId);
        if (payment.Status != PaymentStatus.Pending)
            throw new InvalidOperationException($"Payment with id {paymentId} is not in pending status");
       
        var res = await paymentsRepository.ChangePaymentStatus(paymentId, PaymentStatus.Failed);
        return res;
    }
}