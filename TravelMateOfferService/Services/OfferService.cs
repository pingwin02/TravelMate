using Mapster;
using MassTransit;
using TravelMate.Models.Messages;
using TravelMate.Models.Offers;
using TravelMateOfferService.Models;
using TravelMateOfferService.Models.DTO;
using TravelMateOfferService.Repositories;

namespace TravelMateOfferService.Services;

public class OfferService(IOfferRepository offerRepository,IPublishEndpoint _publishEndpoint) : IOfferService
{

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
        
        var config = new TypeAdapterConfig();
        config.NewConfig<Offer, OfferDto>()
            .Map(dest => dest.AirplaneName, src => src.Airplane.Name)
            .Map(dest => dest.AirlineName, src => src.Airline.Name)
            .Map(dest => dest.AirlineIconUrl, src => src.Airline.IconUrl)
            .Map(dest => dest.DepartureAirportCode, src => src.DepartureAirport.Code)
            .Map(dest => dest.DepartureAirportName, src => src.DepartureAirport.Name)
            .Map(dest => dest.DepartureAirportCity, src => src.DepartureAirport.City)
            .Map(dest => dest.DepartureAirportCountry, src => src.DepartureAirport.Country)
            .Map(dest => dest.ArrivalAirportCode, src => src.ArrivalAirport.Code)
            .Map(dest => dest.ArrivalAirportName, src => src.ArrivalAirport.Name)
            .Map(dest => dest.ArrivalAirportCity, src => src.ArrivalAirport.City)
            .Map(dest => dest.ArrivalAirportCountry, src => src.ArrivalAirport.Country)
            .Map(dest => dest.FlightNumber, src => src.FlightNumber)
            .Map(dest => dest.DepartureTime, src => src.DepartureTime)
            .Map(dest => dest.ArrivalTime, src => src.ArrivalTime)
            .Map(dest => dest.BasePrice, src => src.BasePrice)
            .Map(dest => dest.AvailableEconomySeats, src => src.AvailableEconomySeats)
            .Map(dest => dest.AvailableBusinessSeats, src => src.AvailableBusinessSeats)
            .Map(dest => dest.AvailableFirstClassSeats, src => src.AvailableFirstClassSeats)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt);

        var offerDto = savedOffer.Adapt<OfferDto>(config);
        await _publishEndpoint.Publish(new AddOfferEvent
        {
            Offer = offerDto
        });
        return savedOffer.Id;
    }

    public async Task UpdateOffer(Offer offer)
    {
        await offerRepository.UpdateOffer(offer);
        var config = new TypeAdapterConfig();
        config.NewConfig<Offer, OfferDto>()
            .Map(dest => dest.AirplaneName, src => src.Airplane.Name)
            .Map(dest => dest.AirlineName, src => src.Airline.Name)
            .Map(dest => dest.AirlineIconUrl, src => src.Airline.IconUrl)
            .Map(dest => dest.DepartureAirportCode, src => src.DepartureAirport.Code)
            .Map(dest => dest.DepartureAirportName, src => src.DepartureAirport.Name)
            .Map(dest => dest.DepartureAirportCity, src => src.DepartureAirport.City)
            .Map(dest => dest.DepartureAirportCountry, src => src.DepartureAirport.Country)
            .Map(dest => dest.ArrivalAirportCode, src => src.ArrivalAirport.Code)
            .Map(dest => dest.ArrivalAirportName, src => src.ArrivalAirport.Name)
            .Map(dest => dest.ArrivalAirportCity, src => src.ArrivalAirport.City)
            .Map(dest => dest.ArrivalAirportCountry, src => src.ArrivalAirport.Country)
            .Map(dest => dest.FlightNumber, src => src.FlightNumber)
            .Map(dest => dest.DepartureTime, src => src.DepartureTime)
            .Map(dest => dest.ArrivalTime, src => src.ArrivalTime)
            .Map(dest => dest.BasePrice, src => src.BasePrice)
            .Map(dest => dest.AvailableEconomySeats, src => src.AvailableEconomySeats)
            .Map(dest => dest.AvailableBusinessSeats, src => src.AvailableBusinessSeats)
            .Map(dest => dest.AvailableFirstClassSeats, src => src.AvailableFirstClassSeats)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt);

        var offerDto = offer.Adapt<OfferDto>(config);
        await _publishEndpoint.Publish(new UpdateOfferEvent
        {
            Offer = offerDto
        });
    }

    public async Task DeleteOffer(Guid id)
    {
        await offerRepository.DeleteOffer(id);
        await _publishEndpoint.Publish(new DeleteOfferEvent
        {
            OfferId = id
        });

    }


    public async Task<bool> CheckSeatAvailability(CheckSeatAvailabilityRequest request)
    {
        try
        {
            var offer = await offerRepository.GetOffer(request.OfferId);

            if (offer.DepartureTime < DateTime.Now)
                return false;

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

    public async Task CancelSeatReservation(CancelSeatAvailabilityCommand request)
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

        if (offer.DepartureTime.DayOfWeek == DayOfWeek.Friday || offer.DepartureTime.DayOfWeek == DayOfWeek.Sunday)
            basePrice *= 1.2m;

        const int threshold = 10;
        var availableSeats = request.SeatType switch
        {
            SeatType.Economy => offer.AvailableEconomySeats,
            SeatType.Business => offer.AvailableBusinessSeats,
            SeatType.FirstClass => offer.AvailableFirstClassSeats,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (availableSeats < threshold) basePrice *= 1.1m;

        basePrice *= request.SeatType switch
        {
            SeatType.Economy => 1.0m,
            SeatType.Business => 1.5m,
            SeatType.FirstClass => 2.0m,
            _ => throw new ArgumentOutOfRangeException()
        };

        basePrice *= request.PassengerType switch
        {
            PassengerType.Adult => 1.0m,
            PassengerType.Child => 0.75m,
            PassengerType.Baby => 0.5m,
            _ => throw new ArgumentOutOfRangeException()
        };

        return Math.Round(basePrice, 2, MidpointRounding.AwayFromZero);
    }
}