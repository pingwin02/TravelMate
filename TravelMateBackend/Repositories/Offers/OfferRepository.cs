using Microsoft.EntityFrameworkCore;
using TravelMateBackend.Data;
using TravelMateBackend.Models.Entities;

namespace TravelMateBackend.Repositories.Offers
{
    public class OfferRepository : IOfferRepository
    {
        private readonly DataContext _context;

        public OfferRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Offer> AddOffer(Offer offer)
        {
            await _context.Offers.AddAsync(offer);
            await _context.SaveChangesAsync();
            return offer;
        }

        public async Task<Offer> DeleteOffer(int id)
        {
            var offer = await _context.Offers.FindAsync(id);
            _context.Offers.Remove(offer);
            await _context.SaveChangesAsync();
            return offer;
        }

        public async Task<Offer> GetOffer(Guid id)
        {

            return await _context.Offers.FindAsync(id);
        }

        public async Task<IEnumerable<Offer>> GetOffers()
        {
            var offers = await _context.Offers.Include(x=>x.Airplane)
                                              .Include(x=>x.Airline)
                                              .Include(x=>x.ArrivalAirport)
                                              .Include(x => x.DepartureAirport)
                                              .ToListAsync();
            Console.WriteLine(offers.Count);


            return await _context.Offers.ToListAsync();
        }

        public Task<Offer> UpdateOffer(Offer offer)
        {
            _context.Entry(offer).State = EntityState.Modified;
            _context.SaveChanges();
            return Task.FromResult(offer);
        }

    }
}
