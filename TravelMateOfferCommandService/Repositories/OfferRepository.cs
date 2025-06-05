using Microsoft.EntityFrameworkCore;
using TravelMateOfferCommandService.Data;
using TravelMateOfferCommandService.Models;

namespace TravelMateOfferCommandService.Repositories;

public class OfferRepository(DataContext context) : IOfferRepository
{
    public async Task<IEnumerable<Offer>> GetOffers()
    {
        var offers = await context.Offers
            .Include(x => x.Airplane)
            .Include(x => x.Airline)
            .Include(x => x.ArrivalAirport)
            .Include(x => x.DepartureAirport)
            .ToListAsync();

        return offers;
    }

    public async Task<Offer> GetOffer(Guid id)
    {
        var offer = await context.Offers
            .Include(x => x.Airplane)
            .Include(x => x.Airline)
            .Include(x => x.ArrivalAirport)
            .Include(x => x.DepartureAirport)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (offer == null)
            throw new KeyNotFoundException($"Offer with id {id} not found");
        return offer;
    }


    public async Task<Airline> GetAirlineByName(string name)
    {
        var airline = await context.Airlines.FirstOrDefaultAsync(x => x.Name == name);
        if (airline == null)
            throw new KeyNotFoundException($"Airline with name {name} not found");
        return airline;
    }

    public async Task<Airplane> GetAirplaneById(Guid id)
    {
        var airplane = await context.Airplanes.FirstOrDefaultAsync(x => x.Id == id);
        if (airplane == null)
            throw new KeyNotFoundException($"Airplane with id {id} not found");
        return airplane;
    }

    public async Task<Airport> GetAirportByCode(string code)
    {
        var airport = await context.Airports.FirstOrDefaultAsync(x => x.Code == code);
        if (airport == null)
            throw new KeyNotFoundException($"Airport with code {code} not found");
        return airport;
    }

    public async Task<Offer> AddOffer(Offer offer)
    {
        await context.Offers.AddAsync(offer);
        await context.SaveChangesAsync();
        return offer;
    }

    public async Task UpdateOffer(Offer offer)
    {
        context.Entry(offer).State = EntityState.Modified;
        await context.SaveChangesAsync();
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