using Mapster;
using Models.Entities.Offers;
using TravelMateBackend.Models.Entities.Offers.DTO;
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

        public async Task<IEnumerable<OfferListDto>> GetOffers()
        {
            var offers = await _offerRepository.GetOffers();

            var config = new TypeAdapterConfig();
            config.NewConfig<Offer, OfferListDto>()
                .Map(dest => dest.AirlineName, src => src.Airline.Name)
                .Map(dest => dest.DepartureAirport, src => src.DepartureAirport.Code)
                .Map(dest => dest.ArrivalAirport, src => src.ArrivalAirport.Code);

            return offers.Adapt<IEnumerable<OfferListDto>>(config);
        }

        public async Task<Offer> UpdateOffer(Offer offer)
        {
            return await _offerRepository.UpdateOffer(offer);
        }




    }
}
