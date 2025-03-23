using TravelMateBackend.Models.Entities;
namespace TravelMateBackend.Services.Offers
{
    public interface IOfferService
    {
        Task<IEnumerable<Offer>> GetOffers();
        Task<Offer> GetOffer(Guid id);
        Task<Offer> AddOffer(Offer offer);
        Task<Offer> UpdateOffer(Offer offer);
        Task<Offer> DeleteOffer(int id);
    }
}
