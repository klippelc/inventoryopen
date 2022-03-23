using Inventory.Data.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Inventory.Data.Models
{
    public class Location : BaseEntity
    {
        public int Id { get; set; }

        public string PropertyId { get; set; }

        [MaxLength(300)]
        public string Name { get; set; }

        [MaxLength(300)]
        public string DisplayName { get; set; }

        [MaxLength(10)]
        public string Code { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        [MaxLength(30)]
        public string Phone { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public DbGeography Geography { get; set; }

        [MaxLength(50)]
        public string SubnetAddress { get; set; }

        //Address line 1 should contain the primary address information.
        //Address line 1 should contain the primary address information
        //and secondary address information (e.g., floor, suite or mail stop number) on one line.
        [MaxLength(255)]
        public string AddressLine1 { get; set; }

        //Address line 2 should contain the following, if necessary: Apartment number(Apt 123)
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

        public int? LeadManagerId { get; set; }

        [ForeignKey("LeadManagerId")]
        public User LeadManager { get; set; }

        [DefaultValue(true)]
        public bool Active { get; set; }

        [DefaultValue("false")]
        public bool IsDeleted { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }

        public virtual ICollection<Asset> Assets { get; set; }
        public virtual ICollection<Building> Buildings { get; set; }
        public virtual ICollection<LocationAmenity> LocationAmenities { get; set; }
        public virtual ICollection<LocationAlias> LocationAliases { get; set; }
    }
}