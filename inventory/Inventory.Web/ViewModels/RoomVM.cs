using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Web.ViewModels
{
    public class RoomVM
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

        public string Description { get; set; }

        public int? Capacity { get; set; }

        public int? RoomTypeId { get; set; }

        [Display(Name = "Room Type")]
        public RoomTypeVM RoomType { get; set; }

        public IEnumerable<int> RoomAmenityIds { get; set; }

        public string OriginalRoomAmenityIds { get; set; }

        [Display(Name = "Amenities")]
        public IEnumerable<AmenityVM> RoomAmenities { get; set; }

        public int? LocationId { get; set; }

        public int? OriginalLocationId { get; set; }

        public LocationVM Location { get; set; }

        public int? BuildingId { get; set; }

        public int? OriginalBuildingId { get; set; }

        public BuildingVM Building { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.########}")]
        public decimal? Latitude { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.########}")]
        public decimal? Longitude { get; set; }

        public bool Active { get; set; }

        public string ActiveStatus { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateModified { get; set; }

        //
        [Display(Name = "Location PID")]
        public string LocationPropertyId { get; set; }

        [Display(Name = "Building PID")]
        public string BuildingPropertyId { get; set; }

        public string LocationName { get; set; }

        public string RoomTypeName { get; set; }

        public string RoomAmenityNames { get; set; }

        public int? AssetCount { get; set; }

        public string PreviousUrl { get; set; }
        //

        public IEnumerable<LocationVM> Locations { get; set; }
        public IEnumerable<BuildingVM> Buildings { get; set; }
        public IEnumerable<RoomTypeVM> RoomTypes { get; set; }
        public IEnumerable<AmenityVM> Amenities { get; set; }
    }
}