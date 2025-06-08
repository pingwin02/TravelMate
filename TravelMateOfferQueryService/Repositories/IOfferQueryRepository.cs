using TravelMate.Models.Offers;

namespace TravelMateOfferQueryService.Repositories;

public interface IOfferQueryRepository
{
    Task<OfferDto> GetOffer(Guid id);
    Task<IEnumerable<OfferListDto>> GetOffers();
    Task AddOffer(OfferDto offer);
    Task<bool> UpdateOffer(OfferDto offer);
    Task DeleteOffer(Guid id);
}