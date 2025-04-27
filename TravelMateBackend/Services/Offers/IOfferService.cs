using Models.Entities.Offers;
using TravelMateBackend.Models.Entities.Offers.DTO;
using TravelMate.Models.Messages;

namespace TravelMateBackend.Services.Offers
{
    public interface IOfferService
    {
        Task<IEnumerable<OfferListDto>> GetOffers();
        Task<Offer> GetOffer(Guid id);
        Task<Offer> AddOffer(Offer offer);
        Task<Offer> UpdateOffer(Offer offer);
        Task<Offer> DeleteOffer(int id);
        Task<bool> CheckSeatAvailability(CheckSeatAvailabilityRequest request);
        Task CancelSeatReservation(CancelReservationRequest request);
    }
}
