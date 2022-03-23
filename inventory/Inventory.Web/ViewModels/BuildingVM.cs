using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Web.ViewModels
{
    public class BuildingVM
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

        public int? LocationId { get; set; }

        public int? OriginalLocationId { get; set; }

        public LocationVM Location { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.########}")]
        public decimal? Latitude { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.########}")]
        public decimal? Longitude { get; set; }

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

        public bool Active { get; set; }

        public string ActiveStatus { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        //
        [Display(Name = "Location PID")]
        public string LocationPropertyId { get; set; }

        [Display(Name = "Building PID")]
        public string LocationBuildingPropertyId { get; set; }

        public int? RoomCount { get; set; }

        public int? AssetCount { get; set; }

        public string PreviousUrl { get; set; }
        //

        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateModified { get; set; }

        public IEnumerable<LocationVM> Locations { get; set; }
        public IEnumerable<CityVM> Cities { get; set; }
        public IEnumerable<StateVM> States { get; set; }
    }
}