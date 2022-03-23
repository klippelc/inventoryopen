using Inventory.Data.Common;
using System.ComponentModel;

namespace Inventory.Data.Models
{
    public class LocationAmenity : BaseEntity
    {
        public int Id { get; set; }

        public int LocationId { get; set; }

        public Location Location { get; set; }

        public int AmenityId { get; set; }

        public Amenity Amenity { get; set; }

        [DefaultValue("false")]
        public bool IsDeleted { get; set; }
    }
}
