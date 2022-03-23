using Inventory.Data.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Inventory.Data.Models
{
    public class Room : BaseEntity
    {
        public int Id { get; set; }

        public string PropertyId { get; set; }

        [MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(300)]
        public string DisplayName { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        public int? RoomTypeId { get; set; }

        [ForeignKey("RoomTypeId")]
        public RoomType RoomType { get; set; }

        public int? BuildingId { get; set; }

        [ForeignKey("BuildingId")]
        public Building Building { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public DbGeography Geography { get; set; }

        public int? Capacity { get; set; }

        [DefaultValue(true)]
        public bool Active { get; set; }

        [DefaultValue("false")]
        public bool IsDeleted { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }

        public virtual ICollection<Asset> Assets { get; set; }
        public virtual ICollection<RoomAmenity> RoomAmenities { get; set; }
    }
}