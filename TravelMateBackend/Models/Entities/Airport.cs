using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TravelMateBackend.Models.Entities
{
    public class Airport
    {
        [Key]
        [StringLength(3)]
        public string Code { get; set; }
        public string? Name { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
