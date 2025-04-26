using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TravelMateBackend.Models.Entities
{
    public class Airplane
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int AvailableEconomySeats { get; set; }
        public int AvailableBusinessSeats { get; set; }
        public int AvailableFirstClassSeats { get; set; }


    }
}
