using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelMate.Models.Offers;
using TravelMateOfferQueryService.Models.Offers;

namespace TravelMateOfferQueryService.Services
{
    public interface IOfferQueryService
    {
        Task<OfferDto> GetOffer(Guid id);
        Task<IEnumerable<OfferListDto>> GetOffers();


        //methods for consumers to update the query db
        Task CreateOffer(OfferDto offer);
        Task UpdateOffer(OfferDto offer);
        Task DeleteOffer(Guid id);


    }
}