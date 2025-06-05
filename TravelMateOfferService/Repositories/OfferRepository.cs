using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TravelMateOfferService.Data;
using TravelMateOfferService.Hubs;
using TravelMateOfferService.Models;
using TravelMateOfferService.Models.DTO;

namespace TravelMateOfferService.Repositories;

public class OfferRepository(DataContext context, IHubContext<OfferHub> hubContext) : IOfferRepository
{
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

        await hubContext.Clients.All.SendAsync("OfferAdded", offer);

        return offer;
    }

    public async Task UpdateOffer(Offer offer)
    {
        var oldOffer = await GetOffer(offer.Id);

        context.Entry(offer).State = EntityState.Modified;
        await context.SaveChangesAsync();

        var changeDto = new OfferChangeDto
        {
            OldOffer = oldOffer,
            NewOffer = offer
        };
        //tutaj musi byc publish do OfferQueryService, zeby zaktualizowac oferte w bazie danych
        //await bus.Publish(changeDto);

        await hubContext.Clients.All.SendAsync("OfferUpdated", changeDto);
    }

    public async Task DeleteOffer(Guid id)
    {
        var offer = await context.Offers.FindAsync(id);
        if (offer == null)
            throw new KeyNotFoundException($"Offer with id {id} not found");
        context.Offers.Remove(offer);
        await context.SaveChangesAsync();

        await hubContext.Clients.All.SendAsync("OfferDeleted", id);
    }
}