namespace TravelMate.Messages.Models.Preferences;

public class OfferPreferencesSummaryDto
{
    public List<EnumCountDto> SeatTypeCounts { get; set; }
    public List<EnumCountDto> PassengerTypeCounts { get; set; }
}