using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelMate.Messages.Models.Preferences
{
    public class OfferPreferencesSummaryDto
    {
        public List<EnumCountDto> SeatTypeCounts { get; set; }
        public List<EnumCountDto> PassengerTypeCounts { get; set; }
    }
}
