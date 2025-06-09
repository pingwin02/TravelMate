using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using TravelMate.Models.Messages;

namespace TravelMateBookingService.Models.Bookings.DTO;

public class OfferPreferencesDto
{
    public int AdultCount { get; set; }
    public int ChildCount { get; set; }
    public int InfantCount { get; set; }
    public int EconomyClassCount { get; set; }
    public int BusinessClassCount { get; set; }
    public int FirstClassCount { get; set; }
}
