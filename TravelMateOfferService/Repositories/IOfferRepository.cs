using TravelMateOfferService.Models;

namespace TravelMateOfferService.Repositories;

public interface IOfferRepository
{
    Task<Offer> GetOffer(Guid id);
    Task<IEnumerable<Offer>> GetOffers();
    Task<Guid> AddOffer(Offer offer);
    Task UpdateOffer(Offer offer);
    Task DeleteOffer(Guid id);
}