using Mapster;
using TravelMateOfferService.Models;
using TravelMateOfferService.Models.DTO;
using TravelMateOfferService.Repositories;

namespace TravelMateOfferService.Services
{
    public class OfferService(IOfferRepository offerRepository) : IOfferService
    {
        public async Task<Offer> GetOffer(Guid id)
        {
            return await offerRepository.GetOffer(id);
        }

        public async Task<IEnumerable<OfferListDto>> GetOffers()
        {
            var offers = await offerRepository.GetOffers();

            var config = new TypeAdapterConfig();
            config.NewConfig<Offer, OfferListDto>()
                .Map(dest => dest.AirlineName, src => src.Airline.Name)
                .Map(dest => dest.DepartureAirport, src => src.DepartureAirport.Code)
                .Map(dest => dest.ArrivalAirport, src => src.ArrivalAirport.Code);

            return offers.Adapt<IEnumerable<OfferListDto>>(config);
        }

        public async Task<Guid> AddOffer(Offer offer)
        {
            return await offerRepository.AddOffer(offer);
        }

        public async Task UpdateOffer(Offer offer)
        {
            await offerRepository.UpdateOffer(offer);
        }

        public async Task DeleteOffer(Guid id)
        {
            await offerRepository.DeleteOffer(id);
        }
    }
}