using Mapster;
using Microsoft.EntityFrameworkCore;
using TravelMateOfferService.Data;
using TravelMateOfferService.Models;

namespace TravelMateOfferService.Repositories
{
    public class OfferRepository(DataContext context) : IOfferRepository
    {

        public async Task<Offer> GetOffer(Guid id)
        {
            var offer = await context.Offers
                .Include(x => x.Airplane)
                .Include(x => x.Airline)
                .Include(x => x.ArrivalAirport)
                .Include(x => x.DepartureAirport)
                .FirstOrDefaultAsync(x => x.Id == id);
            
            if (offer == null)
                throw new KeyNotFoundException($"Offer with id {id} not found");
            return offer;
        }

        public async Task<IEnumerable<Offer>> GetOffers()
        {
            var offers = await context.Offers.Include(x => x.Airplane)
                .Include(x => x.Airline)
                .Include(x => x.ArrivalAirport)
                .Include(x => x.DepartureAirport)
                .ToListAsync();
            Console.WriteLine(offers.Count);


            return await context.Offers.ToListAsync();
        }
        
        public async Task<Guid> AddOffer(Offer offer)
        {
            var newOffer = offer.Adapt<Offer>();
            newOffer.Id = Guid.NewGuid();
            await context.Offers.AddAsync(newOffer);
            await context.SaveChangesAsync();
            return newOffer.Id;
        }

        public Task UpdateOffer(Offer offer)
        {
            context.Entry(offer).State = EntityState.Modified;
            context.SaveChanges();
            return Task.CompletedTask;
        }
        
        public async Task DeleteOffer(Guid id)
        {
            var offer = await context.Offers.FindAsync(id);
            if (offer == null)
                throw new KeyNotFoundException($"Offer with id {id} not found");
            context.Offers.Remove(offer);
            await context.SaveChangesAsync();
        }
    }
}