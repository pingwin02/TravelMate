using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using TravelMate.Messages.Models.Preferences;
using TravelMate.Models.Messages;
using TravelMate.Models.Offers;
using TravelMateBookingService.Data;
using TravelMateBookingService.Models.Bookings;
using TravelMateBookingService.Models.Bookings.DTO;

namespace TravelMateBookingService.Repositories;

public class BookingRepository(DataContext dataContext) : IBookingRepository
{
    public async Task<Booking> CreateBooking(Booking booking)
    {
        await dataContext.Bookings.AddAsync(booking);
        await dataContext.SaveChangesAsync();
        await dataContext.BookingEvents.InsertOneAsync(new BookingEvent
        {
            BookingId = booking.Id,
            Offer = new OfferDto { Id = booking.OfferId },
            Status = booking.Status,
            Timestamp = DateTime.UtcNow
        });
        return booking;
    }

    public async Task<bool> ChangeBookingStatus(Guid bookingId, BookingStatus status, OfferDto offer)
    {
        var booking = await dataContext.Bookings.FindAsync(bookingId);

        if (booking == null)
            throw new KeyNotFoundException($"Booking with id {bookingId} not found");

        booking.Status = status;

        dataContext.Bookings.Update(booking);
        await dataContext.SaveChangesAsync();
        await dataContext.BookingEvents.InsertOneAsync(new BookingEvent
        {
            BookingId = booking.Id,
            Offer = offer,
            Status = booking.Status,
            SeatType = booking.SeatType,
            PassengerType = booking.PassengerType,
            Price = booking.Status == BookingStatus.Confirmed ? offer.BasePrice : 0,
            Timestamp = DateTime.UtcNow
        });
        return true;
    }

    public Task<bool> CheckIfPending(Guid bookingId)
    {
        var booking = dataContext.Bookings.Find(bookingId);
        return Task.FromResult(booking is { Status: BookingStatus.Pending });
    }

    public async Task<Booking> GetBookingById(Guid userId, Guid bookingId)
    {
        var booking = await dataContext.Bookings.FindAsync(bookingId);

        if (booking == null || booking.UserId != userId)
            throw new KeyNotFoundException($"Booking with id {bookingId} not found");

        return booking;
    }

    public async Task<List<Booking>> GetBookingsByUserId(Guid userId)
    {
        return await dataContext.Bookings
            .Where(b => b.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<DeparturePreferenceDto>> GetDeparturePreferences()
    {
        var pipeline = new[]
        {
            new BsonDocument("$match", new BsonDocument("Status", 1)),
            new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$Offer.DepartureAirportCode" },
                    { "count", new BsonDocument("$sum", 1) },
                    { "city", new BsonDocument("$first", "$Offer.DepartureAirportCity") },
                    { "country", new BsonDocument("$first", "$Offer.DepartureAirportCountry") }
                }
            ),
            new BsonDocument("$sort", new BsonDocument("count", -1))
        };

        var result = await dataContext.BookingEvents
            .Aggregate<DeparturePreferenceDto>(pipeline)
            .ToListAsync();

        return result;
    }

    public async Task<OfferPreferencesSummaryDto> GetOfferPreferences()
    {
        var seatTypePipeline = new[]
        {
            new BsonDocument("$match", new BsonDocument("Status", 1)),
            new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$SeatType" },
                    { "count", new BsonDocument("$sum", 1) }
                }
            ),
            new BsonDocument("$sort", new BsonDocument("count", -1))
        };

        var seatTypeResult = await dataContext.BookingEvents
            .Aggregate<BsonDocument>(seatTypePipeline)
            .ToListAsync();

        var passengerTypePipeline = new[]
        {
            new BsonDocument("$match", new BsonDocument("Status", 1)),
            new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$PassengerType" },
                    { "count", new BsonDocument("$sum", 1) }
                }
            ),
            new BsonDocument("$sort", new BsonDocument("count", -1))
        };

        var passengerTypeResult = await dataContext.BookingEvents
            .Aggregate<BsonDocument>(passengerTypePipeline)
            .ToListAsync();

        var seatTypeCounts = seatTypeResult.Select(doc => new EnumCountDto
        {
            Type = doc["_id"].IsBsonNull
                ? "Unknown"
                : Enum.IsDefined(typeof(SeatType), doc["_id"].AsInt32)
                    ? ((SeatType)doc["_id"].AsInt32).ToString()
                    : "Unknown",
            Count = doc["count"].AsInt32
        }).ToList();

        var passengerTypeCounts = passengerTypeResult.Select(doc => new EnumCountDto
        {
            Type = doc["_id"].IsBsonNull
                ? "Unknown"
                : Enum.IsDefined(typeof(PassengerType), doc["_id"].AsInt32)
                    ? ((PassengerType)doc["_id"].AsInt32).ToString()
                    : "Unknown",
            Count = doc["count"].AsInt32
        }).ToList();

        return new OfferPreferencesSummaryDto
        {
            SeatTypeCounts = seatTypeCounts,
            PassengerTypeCounts = passengerTypeCounts
        };
    }
}