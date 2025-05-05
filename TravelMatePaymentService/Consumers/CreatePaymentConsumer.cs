using MassTransit;
using TravelMate.Models.Messages;
using TravelMatePaymentService.Services;

namespace TravelMatePaymentService.Consumers;

public class CreatePaymentConsumer(IServiceProvider serviceProvider)
    : IConsumer<PaymentCreationRequest>
{
    public async Task Consume(ConsumeContext<PaymentCreationRequest> context)
    {
        var paymentRequest = context.Message;
        Console.WriteLine($"Received payment request for booking {paymentRequest.BookingId}");

        using var scope = serviceProvider.CreateScope();
        var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

        var payment = await paymentService.CreatePayment(paymentRequest.BookingId, paymentRequest.Price, context.Message.CorrelationId);

        Console.WriteLine($"Payment created for booking {paymentRequest.BookingId}");

        await context.Publish(new PaymentCreatedEvent
        {
            CorrelationId = context.Message.CorrelationId,
            PaymentId = payment.Id
        });
    }
}