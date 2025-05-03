using TravelMateOfferService.Models;

namespace TravelMateOfferService.Repositories;

public interface IOfferRepository
{
    Task<Offer> GetOffer(Guid id);
    Task<IEnumerable<Offer>> GetOffers();
    Task<Airline> GetAirlineByName(string name);
    Task<Airplane> GetAirplaneById(Guid id);
    Task<Airport> GetAirportByCode(string code);
    Task<Offer> AddOffer(Offer offer);
    Task UpdateOffer(Offer offer);
    Task DeleteOffer(Guid id);
}