using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Web.ViewModels
{
    public class AssetBulkVM
    {
        public string AssetIds { get; set; }

        public int? AssetCount { get; set; }

        public int? StatusId { get; set; }

        public int? AssignedUserId { get; set; }

        public int? AssignedAssetId { get; set; }

        public int? ConnectedAssetId { get; set; }

        public int? LocationId { get; set; }

        public int? BuildingId { get; set; }

        public int? RoomId { get; set; }

        [Display(Name = "Surplus Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? SurplusDate { get; set; }

        public string SNFnumber { get; set; }

        [Display(Name = "Date Received")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DateReceived { get; set; }

        public string Notes { get; set; }

        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

        public DateTime? DateAdded { get; set; }
        public DateTime? DateModified { get; set; }

        public IEnumerable<AssetStatusVM> Statuses { get; set; }
        public IEnumerable<LocationVM> Locations { get; set; }
        public IEnumerable<BuildingVM> Buildings { get; set; }
        public IEnumerable<RoomVM> Rooms { get; set; }
        public IEnumerable<AssetVM> ComputerTypeAssets { get; set; }
        public IEnumerable<UserVM> Users { get; set; }
    }
}