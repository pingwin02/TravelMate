using TravelMate.Models.Offers;

namespace TravelMateOfferQueryService.Services;

public interface IOfferQueryService
{
    Task<OfferDto> GetOffer(Guid id);
    Task<IEnumerable<OfferListDto>> GetOffers();


    //methods for consumers to update the query db
    Task AddOffer(OfferDto offer);
    Task UpdateOffer(OfferDto offer);
    Task DeleteOffer(Guid id);
}