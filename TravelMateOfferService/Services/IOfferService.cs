using TravelMate.Models.Messages;
using TravelMate.Models.Offers;
using TravelMateOfferService.Models;
using TravelMateOfferService.Models.DTO;

namespace TravelMateOfferService.Services;

public interface IOfferService
{
    Task<Guid> AddOffer(OfferRequestDto offer);
    Task UpdateOffer(Offer offer);
    Task DeleteOffer(Guid id);
    Task<bool> CheckSeatAvailability(CheckSeatAvailabilityRequest request);
    Task CancelSeatReservation(CancelSeatAvailabilityCommand request);
    Task<decimal> CalculateDynamicPrice(CheckSeatAvailabilityRequest request);
}