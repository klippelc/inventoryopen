using Inventory.Data.Common;
using System.ComponentModel;

namespace Inventory.Data.Models
{
    public class BuildingAmenity : BaseEntity
    {
        public int Id { get; set; }

        public int BuildingId { get; set; }

        public Building Building { get; set; }

        public int AmenityId { get; set; }

        public Amenity Amenity { get; set; }

        [DefaultValue("false")]
        public bool IsDeleted { get; set; }
    }
}
