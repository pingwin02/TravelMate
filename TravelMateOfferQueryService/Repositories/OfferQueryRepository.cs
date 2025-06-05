using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TravelMate.Models.Offers;
using TravelMateOfferQueryService.Data;
using TravelMateOfferQueryService.Models.Offers;

namespace TravelMateOfferQueryService.Repositories
{
    public class OfferQueryRepository(DataContext context) : IOfferQueryRepository
    {
        public Task CreateOffer(OfferDto offer)
        {
            throw new NotImplementedException();
        }

        public Task DeleteOffer(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<OfferDto> GetOffer(Guid id)
        {
            return context.Offers
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new KeyNotFoundException($"Offer with id {id} not found");
        }

        public Task<IEnumerable<OfferListDto>> GetOffers()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateOffer(OfferDto offerDto)
        {
            if (offerDto == null)
            {
                throw new ArgumentNullException(nameof(offerDto), "Offer cannot be null");
            }
            context.Offers.Update(offerDto);
            await context.SaveChangesAsync();
            if (context.Entry(offerDto).State != EntityState.Modified)
            {
                throw new InvalidOperationException($"Failed to update offer with id {offerDto.Id}");
            }
            return true;
        }
    }
}