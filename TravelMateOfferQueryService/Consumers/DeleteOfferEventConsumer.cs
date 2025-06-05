using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using TravelMate.Models.Messages;
using TravelMateOfferQueryService.Services;

namespace TravelMateOfferQueryService.Consumers
{
    public class DeleteOfferEventConsumer(IServiceProvider serviceProvider) : IConsumer<DeleteOfferEvent>
    {
        public async Task Consume(ConsumeContext<DeleteOfferEvent> context)
        {
            var request = context.Message;
            Console.WriteLine("Received Delete Offer Event, Offer id: " + request.OfferId);
            using var scope = serviceProvider.CreateScope();
            var offerQueryService = scope.ServiceProvider.GetRequiredService<IOfferQueryService>();

            await offerQueryService.DeleteOffer(request.OfferId);
            Console.WriteLine("Offer deleted in query db, offer id: " + request.OfferId);
        }
    }
}