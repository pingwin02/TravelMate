using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TravelMate.Models.Offers;
using TravelMateOfferQueryService.Models.Offers;

namespace TravelMateOfferQueryService.Data
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        public DbSet<OfferDto> Offers { get; set; }
    }
}