using Mapster;
using Models.Entities.Offers;
using TravelMateBackend.Models.Entities.Offers.DTO;
using TravelMate.Models.Messages;
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

        public async Task<bool> CheckSeatAvailability(CheckSeatAvailabilityRequest request)
        {
            var offer = await _offerRepository.GetOffer(request.OfferId);
            if (offer == null)
            {
                return false;
            }

            if(request.SeatType == SeatType.Economy)
            {
                if (offer.AvailableEconomySeats > 0)
                {
                    offer.AvailableEconomySeats--;
                    await _offerRepository.UpdateOffer(offer);
                    return true;
                }
            }
            else if (request.SeatType == SeatType.Business)
            {
                if (offer.AvailableBusinessSeats > 0)
                {
                    offer.AvailableBusinessSeats--;
                    await _offerRepository.UpdateOffer(offer);
                    return true;
                }
            }
            else if (request.SeatType == SeatType.FirstClass)
            {
                if (offer.AvailableFirstClassSeats > 0)
                {
                    offer.AvailableFirstClassSeats--;
                    await _offerRepository.UpdateOffer(offer);
                    return true;
                }
            }

            return false;

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

        public async Task CancelSeatReservation(CancelReservationRequest request)
        {
            var offer = await _offerRepository.GetOffer(request.OfferId);
            if (offer == null)
            {
                throw new Exception("Offer not found");
            }
            if (request.SeatType == SeatType.Economy)
            {
                offer.AvailableEconomySeats++;
            }
            else if (request.SeatType == SeatType.Business)
            {
                offer.AvailableBusinessSeats++;
            }
            else if (request.SeatType == SeatType.FirstClass)
            {
                offer.AvailableFirstClassSeats++;
            }
            await _offerRepository.UpdateOffer(offer);
        }

        public async Task<Offer> UpdateOffer(Offer offer)
        {
            return await _offerRepository.UpdateOffer(offer);
        }




    }
}
