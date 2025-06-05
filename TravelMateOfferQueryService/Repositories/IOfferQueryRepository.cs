using TravelMate.Models.Offers;

namespace TravelMateOfferQueryService.Repositories;

public interface IOfferQueryRepository
{
    Task<OfferDto> GetOffer(Guid id);
    Task<IEnumerable<OfferListDto>> GetOffers();

    // methods for consumers to update the query db
    Task AddOffer(OfferDto offer);
    Task<bool> UpdateOffer(OfferDto offer);
    Task DeleteOffer(Guid id);
}