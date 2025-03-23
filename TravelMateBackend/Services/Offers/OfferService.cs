using TravelMateBackend.Models.Entities;
using TravelMateBackend.Repositories.Offers;
namespace TravelMateBackend.Services.Offers
{
    public class OfferService : IOfferService
    {
        private readonly IOfferRepository _offerRepository;


        public OfferService(IOfferRepository offerRepository)
        {
            _offerRepository = offerRepository;
        }

        public async Task<Offer> AddOffer(Offer offer)
        {
            return await _offerRepository.AddOffer(offer);
        }

        public async Task<Offer> DeleteOffer(int id)
        {
            return await _offerRepository.DeleteOffer(id);
        }

        public async Task<Offer> GetOffer(Guid id)
        {
            return await _offerRepository.GetOffer(id);
        }

        public async Task<IEnumerable<Offer>> GetOffers()
        {
            return await _offerRepository.GetOffers();
        }

        public async Task<Offer> UpdateOffer(Offer offer)
        {
            return await _offerRepository.UpdateOffer(offer);
        }




    }
}
