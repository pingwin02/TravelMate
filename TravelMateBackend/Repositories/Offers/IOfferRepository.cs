using TravelMateBackend.Models.Entities;
namespace TravelMateBackend.Repositories.Offers
{
    public interface IOfferRepository
    {
        Task<IEnumerable<Offer>> GetOffers();
        Task<Offer> GetOffer(Guid id);
        Task<Offer> AddOffer(Offer offer);
        Task<Offer> UpdateOffer(Offer offer);
        Task<Offer> DeleteOffer(int id);
    }
}
