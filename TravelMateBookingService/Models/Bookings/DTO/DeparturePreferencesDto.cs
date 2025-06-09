using MongoDB.Bson.Serialization.Attributes;

namespace TravelMateBookingService.Models.Bookings.DTO;

public class DeparturePreferenceDto
{
    [BsonId] public string Id { get; set; }

    [BsonElement("count")] public int Count { get; set; }

    [BsonElement("city")] public string City { get; set; }

    [BsonElement("country")] public string Country { get; set; }
}