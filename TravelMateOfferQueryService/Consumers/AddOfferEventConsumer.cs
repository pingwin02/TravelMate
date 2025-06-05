using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using TravelMate.Models.Messages;
using TravelMateOfferQueryService.Services;

namespace TravelMateOfferQueryService.Consumers
{
     public class AddOfferEventConsumer(IServiceProvider serviceProvider) : IConsumer<AddOfferEvent>
    {
        public async Task Consume(ConsumeContext<AddOfferEvent> context)
        {
            var request = context.Message;
            Console.WriteLine("Received Add Offer Event, Offer id: " + request.Offer.Id);
            using var scope = serviceProvider.CreateScope();
            var offerQueryService = scope.ServiceProvider.GetRequiredService<IOfferQueryService>();

            await offerQueryService.AddOffer(request.Offer);
            Console.WriteLine("Offer added in query db, offer id: " + request.Offer.Id);
        }
    }
}