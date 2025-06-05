namespace TravelMate.Models.Offers;

public class OfferChangeDto
{
    public OfferDto? OldOffer { get; set; }
    public OfferDto? NewOffer { get; set; }
}