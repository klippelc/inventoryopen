using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Data.Models
{
    public class LocationAlias 
    {
        public int Id { get; set; }

        public int? LocationId { get; set; }

        [ForeignKey("LocationId")]
        public Location Location { get; set; }

        [MaxLength(300)]
        public string Name { get; set; }
    }
}
