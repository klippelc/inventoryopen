using Inventory.Data.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Data.Models
{
    public class Amenity : BaseEntity
    {
        public int Id { get; set; }

        [MaxLength(150)]
        public string Name { get; set; }

        public int Sequence { get; set; }

        public int? TypeId { get; set; }

        [ForeignKey("TypeId")]
        public AmenityType Type { get; set; }

        [DefaultValue(true)]
        public bool Active { get; set; }

        public virtual ICollection<RoomAmenity> RoomAmenities { get; set; }
        public virtual ICollection<BuildingAmenity> BuildingAmenities { get; set; }
        public virtual ICollection<LocationAmenity> LocationAmenities { get; set; }
    }
}
