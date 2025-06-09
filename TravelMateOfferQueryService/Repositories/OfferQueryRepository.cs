using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using TravelMate.Models.Offers;
using TravelMateOfferQueryService.Hubs;

namespace TravelMateOfferQueryService.Repositories;

public class OfferQueryRepository(DataContext context, IHubContext<OfferHub> offerHubContext, IHubContext<OfferChangesHub> offerChangesHubContext) : IOfferQueryRepository
{
    public async Task AddOffer(OfferDto offer)
    {
        if (offer == null) throw new ArgumentNullException(nameof(offer), "Offer cannot be null");

        await context.Offers.InsertOneAsync(offer);
        await offerHubContext.Clients.Group(offer.Id.ToString())
            .SendAsync("OfferAdded", offer);
        await offerChangesHubContext.Clients.All
            .SendAsync("OfferAdded", offer);
    }

    public async Task DeleteOffer(Guid id)
    {
        var filter = Builders<OfferDto>.Filter.Eq(o => o.Id, id);
        var result = await context.Offers.DeleteOneAsync(filter);

        if (result.DeletedCount > 0)
        {
            await offerHubContext.Clients.Group(id.ToString())
                .SendAsync("OfferDeleted", id);
            await offerChangesHubContext.Clients.All.SendAsync("OfferDeleted", id);
        }
        else
            throw new Exception($"Offer with ID {id} was not found.");
    }

    public async Task<OfferDto> GetOffer(Guid id)
    {
        return await context.Offers.Find(x => x.Id == id)
                   .FirstOrDefaultAsync()
               ?? throw new KeyNotFoundException($"Offer with id {id} not found");
    }

    public async Task<IEnumerable<OfferListDto>> GetOffers()
    {
        var offers = await context.Offers
            .Find(_ => true)
            .Project(x => new OfferListDto
            {
                Id = x.Id,
                AirlineName = x.AirlineName,
                FlightNumber = x.FlightNumber,
                DepartureAirport = x.DepartureAirportCode,
                ArrivalAirport = x.ArrivalAirportCode,
                DepartureCity = x.DepartureAirportCity,
                ArrivalCity = x.ArrivalAirportCity,
                DepartureTime = x.DepartureTime,
                ArrivalTime = x.ArrivalTime,
                BasePrice = x.BasePrice
            })
            .ToListAsync();

        return offers;
    }

    public async Task<bool> UpdateOffer(OfferDto offerDto)
    {
        if (offerDto == null) throw new ArgumentNullException(nameof(offerDto), "Offer cannot be null");
        var oldOffer = await GetOffer(offerDto.Id);
        var filter = Builders<OfferDto>.Filter.Eq(o => o.Id, offerDto.Id);
        var result = await context.Offers.ReplaceOneAsync(filter, offerDto);

        if (result.ModifiedCount == 0)
            throw new InvalidOperationException($"Failed to update offer with id {offerDto.Id}");

        var offerChange = new OfferChangeDto
        {
            OldOffer = oldOffer,
            NewOffer = offerDto
        };
        await offerHubContext.Clients.Group(offerDto.Id.ToString())
            .SendAsync("OfferUpdated", offerChange);

        await offerChangesHubContext.Clients.All
            .SendAsync("OfferUpdated", offerChange);
        return true;
    }
}