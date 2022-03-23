using Inventory.Data.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Inventory.Data.Models
{
    public class Building : BaseEntity
    {
        public int Id { get; set; }

        public string PropertyId { get; set; }

        [MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(300)]
        public string DisplayName { get; set; }

        [MaxLength(10)]
        public string Code { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        [MaxLength(30)]
        public string Phone { get; set; }

        public int? LocationId { get; set; }

        [ForeignKey("LocationId")]
        public Location Location { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public DbGeography Geography { get; set; }

        [MaxLength(255)]
        public string AddressLine1 { get; set; }

        [MaxLength(255)]
        public string AddressLine2 { get; set; }

        public int? CityId { get; set; }

        [ForeignKey("CityId")]
        public City City { get; set; }

        public int? StateId { get; set; }

        [ForeignKey("StateId")]
        public State State { get; set; }

        [MaxLength(20)]
        public string PostalCode { get; set; }

        [DefaultValue(true)]
        public bool Active { get; set; }

        [DefaultValue("false")]
        public bool IsDeleted { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<Asset> Assets { get; set; }
        public virtual ICollection<BuildingAmenity> BuildingAmenities { get; set; }
    }
}