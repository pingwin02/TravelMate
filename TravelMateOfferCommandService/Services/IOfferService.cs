using TravelMate.Models.Messages;
using TravelMate.Models.Offers;
using TravelMateOfferCommandService.Models;
using TravelMateOfferCommandService.Models.DTO;

namespace TravelMateOfferCommandService.Services;

public interface IOfferService
{
    Task<IEnumerable<OfferListDto>> GetOffers();
    Task<Offer> GetOffer(Guid id);
    Task<Guid> AddOffer(OfferRequestDto offer);
    Task UpdateOffer(Offer offer);
    Task DeleteOffer(Guid id);
    Task<bool> CheckSeatAvailability(CheckSeatAvailabilityRequest request);
    Task CancelSeatReservation(CancelSeatAvailabilityCommand request);
    Task<decimal> CalculateDynamicPrice(CheckSeatAvailabilityRequest request);
}