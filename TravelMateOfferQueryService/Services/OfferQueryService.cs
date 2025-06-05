using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TravelMate.Models.Offers;
using TravelMateOfferQueryService.Hubs;
using TravelMateOfferQueryService.Repositories;

namespace TravelMateOfferQueryService.Services
{
    public class OfferQueryService(IOfferQueryRepository offerQueryRepository) : IOfferQueryService
    {
        public Task AddOffer(OfferDto offer)
        {
            if (offer == null)
            {
                throw new ArgumentNullException(nameof(offer), "Offer cannot be null");
            }

            return offerQueryRepository.AddOffer(offer);
        }

        public async Task DeleteOffer(Guid id)
        {
            await offerQueryRepository.DeleteOffer(id);
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
          
        }
    }
}