using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TravelMate.Models.Offers;
using TravelMateOfferQueryService.Hubs;
using TravelMateOfferQueryService.Models.Offers;
using TravelMateOfferQueryService.Repositories;

namespace TravelMateOfferQueryService.Services
{
    public class OfferQueryService(IOfferQueryRepository offerQueryRepository, IHubContext<OfferHub> hubContext) : IOfferQueryService
    {
        public Task CreateOffer(OfferDto offer)
        {
            throw new NotImplementedException();
        }

        public Task DeleteOffer(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<OfferDto> GetOffer(Guid id)
        {
            return await offerQueryRepository.GetOffer(id);
        }

        public async Task<IEnumerable<OfferListDto>> GetOffers()
        {
            return await offerQueryRepository.GetOffers();
        }

        public async Task UpdateOffer(OfferDto offer)
        {
            if (offer == null)
            {
                throw new ArgumentNullException(nameof(offer), "Offer cannot be null");
            }

            var result = await offerQueryRepository.UpdateOffer(offer);
            if (!result)
            {
                throw new InvalidOperationException($"Failed to update offer with id {offer.Id}");
            }
            await hubContext.Clients.All.SendAsync("OfferUpdated", offer);
        }
    }
}