using Mapster;
using TravelMate.Models.Messages;
using TravelMateOfferService.Models;
using TravelMateOfferService.Models.DTO;
using TravelMateOfferService.Repositories;

namespace TravelMateOfferService.Services;

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

    public async Task<Guid> AddOffer(OfferRequestDto newOffer)
    {
        var airplane = await offerRepository.GetAirplaneById(newOffer.AirplaneId);
        var airline = await offerRepository.GetAirlineByName(newOffer.AirlineName);
        var departureAirport = await offerRepository.GetAirportByCode(newOffer.DepartureAirportCode);
        var arrivalAirport = await offerRepository.GetAirportByCode(newOffer.ArrivalAirportCode);

        var offer = new Offer
        {
            Airplane = airplane,
            Airline = airline,
            DepartureAirport = departureAirport,
            ArrivalAirport = arrivalAirport,
            FlightNumber = newOffer.FlightNumber,
            DepartureTime = newOffer.DepartureTime,
            ArrivalTime = newOffer.ArrivalTime,
            BasePrice = newOffer.BasePrice,
            AvailableEconomySeats = newOffer.AvailableEconomySeats,
            AvailableBusinessSeats = newOffer.AvailableBusinessSeats,
            AvailableFirstClassSeats = newOffer.AvailableFirstClassSeats,
            CreatedAt = DateTime.Now
        };

        var savedOffer = await offerRepository.AddOffer(offer);
        return savedOffer.Id;
    }

    public async Task UpdateOffer(Offer offer)
    {
        await offerRepository.UpdateOffer(offer);
    }

    public async Task DeleteOffer(Guid id)
    {
        await offerRepository.DeleteOffer(id);
    }


    public async Task<bool> CheckSeatAvailability(CheckSeatAvailabilityRequest request)
    {
        try
        {
            var offer = await offerRepository.GetOffer(request.OfferId);

            switch (request.SeatType)
            {
                case SeatType.Economy when offer.AvailableEconomySeats > 0:
                    offer.AvailableEconomySeats--;
                    break;
                case SeatType.Business when offer.AvailableBusinessSeats > 0:
                    offer.AvailableBusinessSeats--;
                    break;
                case SeatType.FirstClass when offer.AvailableFirstClassSeats > 0:
                    offer.AvailableFirstClassSeats--;
                    break;
                default:
                    return false;
            }

            await offerRepository.UpdateOffer(offer);
            return true;
        }
        catch (KeyNotFoundException)
        {
            return false;
        }
    }

    public async Task CancelSeatReservation(CancelReservationRequest request)
    {
        var offer = await offerRepository.GetOffer(request.OfferId);

        switch (request.SeatType)
        {
            case SeatType.Economy:
                offer.AvailableEconomySeats++;
                break;
            case SeatType.Business:
                offer.AvailableBusinessSeats++;
                break;
            case SeatType.FirstClass:
                offer.AvailableFirstClassSeats++;
                break;
        }

        await offerRepository.UpdateOffer(offer);
    }

    public async Task<decimal> CalculateDynamicPrice(CheckSeatAvailabilityRequest request)
    {
        var offer = await offerRepository.GetOffer(request.OfferId);
        var basePrice = offer.BasePrice;

        switch (request.PassengerType)
        {
            // TODO: Add more complex logic for dynamic pricing
            case PassengerType.Adult:
                basePrice *= 1.2m; // Increase price for adults
                break;
            case PassengerType.Child:
                basePrice *= 0.8m; // Decrease price for children
                break;
            case PassengerType.Baby:
                basePrice *= 0.5m; // Decrease price for babies
                break;
        }

        return basePrice;
    }
}