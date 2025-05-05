using MassTransit;
using TravelMate.Models.Messages;
using TravelMatePaymentService.Services;

namespace TravelMatePaymentService.Consumers
{
    public class CancelPaymentConsumer(IServiceProvider serviceProvider) : IConsumer<CancelPaymentCommand>
    {
        public async Task Consume(ConsumeContext<CancelPaymentCommand> context)
        {
            var request = context.Message;
            Console.WriteLine("Received CancelPaymentCommand: " + request.PaymentId);
            using var scope = serviceProvider.CreateScope();
            var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

            var res = await paymentService.CancelPayment(request.PaymentId);

            Console.WriteLine($"Payment cancellation for: {request.PaymentId} - success: {res}");

        }
    }
}
