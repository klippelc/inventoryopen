using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Web.ViewModels
{
    public class LocationVM
    {
        public int Id { get; set; }

        [RegularExpression("^[0-9]{1,4}$", ErrorMessage = "Must be numeric")]
        [Display(Name = "Property Id")]
        public string PropertyId { get; set; }

        public string OriginalPropertyId { get; set; }

        [RegularExpression("^[a-zA-Z0-9' '_/&.-]*$", ErrorMessage = "Only AlphaNumeric, and '&-/.' allowed")]
        public string Name { get; set; }

        public string OriginalName { get; set; }

        [RegularExpression("^[a-zA-Z0-9' '_/&.-]*$", ErrorMessage = "Only AlphaNumeric, and '&-/.' allowed")]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        public string OriginalDisplayName { get; set; }

        public string Code { get; set; }

        public string OriginalCode { get; set; }

        public string Description { get; set; }

        [Phone]
        public string Phone { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.########}")]
        public decimal? Latitude { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.########}")]
        public decimal? Longitude { get; set; }

        [RegularExpression(@"^(([1-9]?\d|1\d\d|2[0-5][0-5]|2[0-4]\d)\.){3}([1-9]?\d|1\d\d|2[0-5][0-5]|2[0-4]\d)$", ErrorMessage = "Format must be 0-255.0-255.0-255.0-255")]
        [Display(Name = "Subnet Address")]
        public string SubnetAddress { get; set; }

        public string OriginalSubnetAddress { get; set; }

        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        public int? CityId { get; set; }

        public CityVM City { get; set; }

        public int? StateId { get; set; }

        public StateVM State { get; set; }

        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Display(Name = "Amenities")]
        public IEnumerable<AmenityVM> LocationAmenities { get; set; }

        public IEnumerable<int> LocationAmenityIds { get; set; }

        public string OriginalLocationAmenityIds { get; set; }

        public string LocationAmenityNames { get; set; }

        [Display(Name = "Aliases")]
        public IEnumerable<LocationAliasVM> LocationAliases { get; set; }

        [Display(Name = "Aliases")]
        public string LocationAliasNames { get; set; }

        public string OriginalLocationAliasNames { get; set; }

        public bool Active { get; set; }

        public string ActiveStatus { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public int? LeadManagerId { get; set; }

        [Display(Name = "Lead Manager")]
        public UserVM LeadManager { get; set; }

        //
        public int? BuildingCount { get; set; }

        public int? RoomCount { get; set; }

        public int? AssetCount { get; set; }

        public string PreviousUrl { get; set; }

        //

        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateModified { get; set; }

        public IEnumerable<CityVM> Cities { get; set; }
        public IEnumerable<StateVM> States { get; set; }
        public IEnumerable<UserVM> Users { get; set; }
        public IEnumerable<AmenityVM> Amenities { get; set; }
        public IEnumerable<LocationAliasVM> Aliases { get; set; }

    }
}