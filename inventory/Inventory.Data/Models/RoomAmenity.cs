using Inventory.Data.Common;
using System.ComponentModel;

namespace Inventory.Data.Models
{
    public class RoomAmenity : BaseEntity
    {
        public int Id { get; set; }

        public int RoomId { get; set; }

        public Room Room { get; set; }

        public int AmenityId { get; set; }

        public Amenity Amenity { get; set; }

        [DefaultValue("false")]
        public bool IsDeleted { get; set; }
    }
}
