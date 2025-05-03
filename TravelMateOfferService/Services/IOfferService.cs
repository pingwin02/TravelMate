using TravelMateOfferService.Models;
using TravelMateOfferService.Models.DTO;

namespace TravelMateOfferService.Services;

public interface IOfferService
{
    Task<Offer> GetOffer(Guid id);
    Task<IEnumerable<OfferListDto>> GetOffers();
    Task<Guid> AddOffer(Offer offer);
    Task UpdateOffer(Offer offer);
    Task DeleteOffer(Guid id);
}